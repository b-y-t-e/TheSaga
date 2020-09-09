using System;
using System.Linq;
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
        private readonly ISagaPersistance sagaPersistance;
        private readonly IServiceProvider serviceProvider;

        public ExecuteStepCommandHandler(
            ISagaPersistance sagaPersistance,
            IServiceProvider serviceProvider,
            IDateTimeProvider dateTimeProvider)
        {
            this.sagaPersistance = sagaPersistance;
            this.serviceProvider = serviceProvider;
            this.dateTimeProvider = dateTimeProvider;
        }

        public async Task<ISaga> Handle(ExecuteStepCommand command)
        {
            ISaga saga = command.Saga;
            ISagaStep sagaStep = command.SagaStep;
            ISagaAction sagaAction = command.SagaAction;
            ISagaEvent @event = command.Event;
            Exception executionError = null;

            saga.State.CurrentStep = sagaStep.StepName;
            saga.Info.Modified = dateTimeProvider.Now;

            StepData executionData = saga.State.History.
                Create(saga, sagaStep);

            executionData.
                MarkStarted(saga.State, dateTimeProvider);

            await sagaPersistance.Set(saga);

            try
            {
                Type executionContextType =
                    typeof(ExecutionContext<>).ConstructGenericType(saga.Data.GetType());

                IExecutionContext context = (IExecutionContext)ActivatorUtilities.CreateInstance(serviceProvider,
                    executionContextType, saga.Data, saga.Info, saga.State);

                if (@event is EmptyEvent)
                    @event = null;

                if (saga.State.IsResuming)
                {
                    await sagaStep.Compensate(context, @event);
                }
                else if (saga.State.IsCompensating)
                {
                    await sagaStep.Compensate(context, @event);
                }
                else
                {
                    await sagaStep.Execute(context, @event);
                }

                executionData.
                    MarkSucceeded(saga.State, dateTimeProvider);
            }
            catch (SagaStopException)
            {
                return null;
            }
            catch (Exception ex)
            {
                executionError = ex;

                executionData.
                    MarkFailed(saga.State, dateTimeProvider, executionError.ToSagaStepException());
            }
            finally
            {
                executionData.
                    MarkEnded(saga.State, dateTimeProvider);
            }

            string nextStepName = null;
            if (executionError != null)
            {
                saga.State.IsResuming = false;
                saga.State.IsCompensating = true;
                saga.State.CurrentError = executionError.ToSagaStepException();
                nextStepName = CalculateNextStep(saga, sagaAction, sagaStep);
            }
            else
            {
                nextStepName = CalculateNextStep(saga, sagaAction, sagaStep);
                saga.State.IsResuming = false;
            }

            executionData.
                SetNextStepName(nextStepName);

            executionData.
                SetEndStateName(saga.State.CurrentState);

            // czy ostatni krok
            if (nextStepName == null)
            {
                saga.State.IsCompensating = false;
                saga.State.CurrentStep = null;
            }
            else
            {
                saga.State.CurrentStep = nextStepName;
            }

            saga.Info.Modified = dateTimeProvider.Now;

            await sagaPersistance.Set(saga);

            return saga;
        }

        private string CalculateNextStep(ISaga saga, ISagaAction sagaAction, ISagaStep sagaStep)
        {
            if (saga.State.IsResuming)
            {
                return sagaStep.StepName;
            }

            if (saga.State.IsCompensating)
            {
                StepData latestToCompensate = saga.State.History.
                    GetNextToCompensate(saga.State.ExecutionID);

                if (latestToCompensate != null)
                    return latestToCompensate.StepName;

                return null;
            }

            return sagaAction.
                GetNextStep(sagaStep)?.StepName;
        }

    }
}