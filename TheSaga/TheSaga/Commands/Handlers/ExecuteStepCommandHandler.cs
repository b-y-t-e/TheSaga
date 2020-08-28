using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.ExecutionContext;
using TheSaga.MessageBus;
using TheSaga.Messages;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Providers;
using TheSaga.SagaModels;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.History;
using TheSaga.SagaModels.Steps;
using TheSaga.Utils;
using TheSaga.ValueObjects;

namespace TheSaga.Commands.Handlers
{
    internal class ExecuteStepCommandHandler
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IMessageBus internalMessageBus;
        private readonly ISagaPersistance sagaPersistance;
        private readonly IServiceProvider serviceProvider;

        public ExecuteStepCommandHandler(
            ISagaPersistance sagaPersistance,
            IServiceProvider serviceProvider, IDateTimeProvider dateTimeProvider,
            IMessageBus internalMessageBus)
        {
            this.sagaPersistance = sagaPersistance;
            this.serviceProvider = serviceProvider;
            this.dateTimeProvider = dateTimeProvider;
            this.internalMessageBus = internalMessageBus;
        }

        public async Task<ISaga> Handle(ExecuteStepCommand command)
        {
            if (command.Async)
            {
                ExecuteStepAsync(command);
                return null;
            }

            return await ExecuteStepSync(command);
        }

        private string CalculateNextStep(ISaga saga, ISagaAction sagaAction, ISagaStep sagaStep)
        {
            if (saga.State.IsCompensating)
            {
                StepData latestToCompensate = saga.State.History.GetLatestToCompensate(saga.State.ExecutionID);

                if (latestToCompensate != null)
                    return latestToCompensate.StepName;

                return null;
            }

            return sagaAction.FindNextAfter(sagaStep)?.StepName;
        }

        private void ExecuteStepAsync(ExecuteStepCommand command)
        {
            Task.Run(() => ExecuteStepSync(command));
        }

        private async Task<ISaga> ExecuteStepSync(ExecuteStepCommand command)
        {
            ISaga saga = command.Saga;
            ISagaStep sagaStep = command.SagaStep;
            AsyncExecution async = command.Async;
            ISagaAction sagaAction = command.SagaAction;
            IEvent @event = command.Event;
            ISagaModel model = command.Model;

            string currentSagaState = saga.State.CurrentState;
            bool hasSagaCompleted = false;

            saga.State.CurrentStep = sagaStep.StepName;
            saga.Info.Modified = dateTimeProvider.Now;

            StepData executionData = saga.State.History.PrepareExecutionData(saga, async, dateTimeProvider);

            await sagaPersistance.Set(saga);

            try
            {
                Type executionContextType =
                    typeof(ExecutionContext<>).ConstructGenericType(saga.Data.GetType());

                IExecutionContext context = (IExecutionContext) ActivatorUtilities.CreateInstance(serviceProvider,
                    executionContextType, saga.Data, saga.Info, saga.State);

                if (@event is EmptyEvent)
                    @event = null;

                if (saga.State.IsCompensating)
                    await sagaStep.Compensate(context, @event);
                else
                    await sagaStep.Execute(context, @event);

                executionData.Succeeded(dateTimeProvider);
            }
            catch (SagaStopException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                saga.State.IsCompensating = true;
                saga.State.CurrentError = ex.ToSagaStepException();

                executionData.Failed(dateTimeProvider, ex.ToSagaStepException());
            }
            finally
            {
                executionData.Ended(dateTimeProvider);
            }

            string nextStepName = CalculateNextStep(saga, sagaAction, sagaStep);

            executionData.SetNextStepName(nextStepName);

            executionData.SetEndStateName(saga.State.CurrentState);

            if (nextStepName != null)
            {
                saga.State.CurrentStep = nextStepName;
            }
            else
            {
                hasSagaCompleted = true;
                saga.State.IsCompensating = false;
                saga.State.CurrentStep = null;
            }

            saga.Info.Modified = dateTimeProvider.Now;

            await sagaPersistance.Set(saga);

            await SendInternalMessages(saga, model, currentSagaState, hasSagaCompleted, async);

            ThrowErrorIfSagaCompletedForSyncCall(saga, async);

            return saga;
        }

        private async Task SendInternalMessages(
            ISaga saga, ISagaModel model, string currentState, bool hasSagaCompleted, AsyncExecution async)
        {
            if (currentState != saga.State.CurrentState)
                await internalMessageBus.Publish(
                    new StateChangedMessage(SagaID.From(saga.Data.ID), saga.State.CurrentState, saga.State.CurrentStep,
                        saga.State.IsCompensating));

            if (hasSagaCompleted)
            {
                await internalMessageBus.Publish(
                    new ExecutionEndMessage(saga));
            }
            else
            {
                if (async)
                    await internalMessageBus.Publish(
                        new AsyncStepCompletedMessage(SagaID.From(saga.Data.ID), saga.State.CurrentState,
                            saga.State.CurrentStep, saga.State.IsCompensating, model));
            }
        }

        private void ThrowErrorIfSagaCompletedForSyncCall(ISaga saga, AsyncExecution async)
        {
            if (async)
                return;

            if (saga.IsIdle() &&
                saga.HasError())
                throw saga.State.CurrentError;
        }
    }
}