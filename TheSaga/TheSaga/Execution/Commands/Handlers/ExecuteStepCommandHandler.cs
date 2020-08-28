using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Execution.Actions;
using TheSaga.Execution.Context;
using TheSaga.InternalMessages;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Providers;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;
using TheSaga.Utils;

namespace TheSaga.Execution.Commands
{
    internal class ExecuteStepCommandHandler<TSagaData>
        where TSagaData : ISagaData
    {
        private ISagaPersistance sagaPersistance;
        private IServiceProvider serviceProvider;
        private IDateTimeProvider dateTimeProvider;
        private IInternalMessageBus internalMessageBus;

        public ExecuteStepCommandHandler(
            ISagaPersistance sagaPersistance,
            IServiceProvider serviceProvider, IDateTimeProvider dateTimeProvider, IInternalMessageBus internalMessageBus)
        {
            this.sagaPersistance = sagaPersistance;
            this.serviceProvider = serviceProvider;
            this.dateTimeProvider = dateTimeProvider;
            this.internalMessageBus = internalMessageBus;
        }

        public async Task Handle(ExecuteStepCommand<TSagaData> command)
        {
            if (command.async)
            {
                ExecuteStepAsync(command);
            }
            else
            {
                await ExecuteStepSync(command);
            }
        }

        private void ExecuteStepAsync(ExecuteStepCommand<TSagaData> command)
        {
            Task.Run(() => ExecuteStepSync(command));
        }

        private async Task ExecuteStepSync(ExecuteStepCommand<TSagaData> command)
        {
            var saga = command.saga;
            var sagaStep = command.sagaStep;
            var async = command.async;
            var sagaAction = command.sagaAction;
            var @event = command.@event;

            string currentSagaState = saga.State.CurrentState;
            bool hasSagaCompleted = false;

            saga.State.CurrentStep = sagaStep.StepName;
            saga.Info.Modified = dateTimeProvider.Now;

            StepData executionData = saga.State.History.
                PrepareExecutionData(saga, async, dateTimeProvider);

            await sagaPersistance.
                Set(saga);

            try
            {
#if DEBUG
                Console.WriteLine($"state: {saga.State.CurrentState}; step: {saga.State.CurrentStep}; action: {(saga.State.IsCompensating ? "Compensate" : "Execute")}");
#endif

                IExecutionContext context = new ExecutionContext<TSagaData>((TSagaData)saga.Data, saga.Info, saga.State);

                if (@event is EmptyEvent)
                    @event = null;

                if (saga.State.IsCompensating)
                {
                    await sagaStep.
                        Compensate(context, @event);
                }
                else
                {
                    await sagaStep.
                        Execute(context, @event);
                }

                executionData.
                    Succeeded(dateTimeProvider);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                saga.State.IsCompensating = true;
                saga.State.CurrentError = ex.ToSagaStepException();

                executionData.
                    Failed(dateTimeProvider, ex.ToSagaStepException());
            }
            finally
            {
                executionData.
                    Ended(dateTimeProvider);
            }

            String nextStepName = CalculateNextStep(saga, sagaAction, sagaStep);

            executionData.
                SetNextStepName(nextStepName);

            executionData.
                SetEndStateName(saga.State.CurrentState);

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

            await sagaPersistance.
                Set(saga);

            await SendInternalMessages(saga, currentSagaState, hasSagaCompleted, async);

            ThrowErrorIfSagaCompletedForSyncCall(saga, async);
        }

        private String CalculateNextStep(ISaga saga, ISagaAction sagaAction, ISagaStep sagaStep)
        {
            if (saga.State.IsCompensating)
            {
                StepData latestToCompensate = saga.State.History.
                    GetLatestToCompensate(saga.State.ExecutionID);

                if (latestToCompensate != null)
                    return latestToCompensate.StepName;

                return null;
            }
            else
            {
                return sagaAction.
                    FindNextAfter(sagaStep)?.
                    StepName;
            }
        }

        private void ThrowErrorIfSagaCompletedForSyncCall(ISaga saga, IsExecutionAsync async)
        {
            if (@async)
                return;

            if (saga.IsIdle() &&
                saga.HasError())
            {
                throw saga.State.CurrentError;
            }
        }

        private async Task SendInternalMessages(
            ISaga saga, string currentState, bool hasSagaCompleted, IsExecutionAsync async)
        {
            if (currentState != saga.State.CurrentState)
            {
                await internalMessageBus.Publish(
                    new SagaStateChangedMessage(typeof(TSagaData), SagaID.From(saga.Data.ID), saga.State.CurrentState, saga.State.CurrentStep, saga.State.IsCompensating));
            }

            if (hasSagaCompleted)
            {
                await internalMessageBus.Publish(
                    new SagaExecutionEndMessage(saga));
            }
            else
            {
                if (@async)
                {
                    await internalMessageBus.Publish(
                        new SagaAsyncStepCompletedMessage(typeof(TSagaData), SagaID.From(saga.Data.ID), saga.State.CurrentState, saga.State.CurrentStep, saga.State.IsCompensating));
                }
            }
        }
    }
}