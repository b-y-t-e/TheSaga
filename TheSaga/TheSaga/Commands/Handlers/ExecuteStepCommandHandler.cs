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
            ISagaModel model = command.Model;
            Exception executionError = null;

            saga.ExecutionState.CurrentStep = sagaStep.StepName;
            saga.ExecutionInfo.Modified = dateTimeProvider.Now;

            StepData stepData = saga.ExecutionState.History.
                Create(saga, sagaStep, model);

            stepData.
                MarkStarted(saga.ExecutionState, dateTimeProvider);

            await sagaPersistance.Set(saga);

            try
            {
                Type executionContextType =
                    typeof(ExecutionContext<>).ConstructGenericType(saga.Data.GetType());

                IExecutionContext context = (IExecutionContext)ActivatorUtilities.CreateInstance(serviceProvider,
                    executionContextType, saga.Data, saga.ExecutionInfo, saga.ExecutionState, saga.ExecutionValues);

                if (@event is EmptyEvent)
                    @event = null;

                if (saga.ExecutionState.IsResuming)
                {
                    await sagaStep.Compensate(serviceProvider, context, @event, stepData);
                }
                else if (saga.ExecutionState.IsCompensating)
                {
                    await sagaStep.Compensate(serviceProvider, context, @event, stepData);
                }
                else
                {
                    await sagaStep.Execute(serviceProvider, context, @event, stepData);
                }

                stepData.
                    MarkSucceeded(saga.ExecutionState, dateTimeProvider);
            }
            catch (SagaStopException)
            {
                return null;
            }
            catch (Exception ex)
            {
                executionError = ex;

                stepData.
                    MarkFailed(saga.ExecutionState, dateTimeProvider, executionError.ToSagaStepException());
            }
            finally
            {
                stepData.
                    MarkEnded(saga.ExecutionState, dateTimeProvider);
            }

            string nextStepName = null;
            if (executionError != null)
            {
                saga.ExecutionState.IsResuming = false;
                saga.ExecutionState.IsCompensating = true;
                saga.ExecutionState.CurrentError = executionError.ToSagaStepException();
                nextStepName = CalculateNextStep(saga, sagaAction, sagaStep);
            }
            else
            {
                nextStepName = CalculateNextStep(saga, sagaAction, sagaStep, stepData);
                saga.ExecutionState.IsResuming = false;
            }

            stepData.
                SetNextStepName(nextStepName);

            stepData.
                SetEndStateName(saga.ExecutionState.CurrentState);

            // czy ostatni krok
            if (nextStepName == null)
            {
                saga.ExecutionState.IsCompensating = false;
                saga.ExecutionState.CurrentStep = null;
            }
            else
            {
                saga.ExecutionState.CurrentStep = nextStepName;
            }

            saga.ExecutionInfo.Modified = dateTimeProvider.Now;

            await sagaPersistance.Set(saga);

            return saga;
        }

        private string CalculateNextStep(ISaga saga, ISagaAction sagaAction, ISagaStep sagaStep, IStepData stepData = null)
        {
            if (saga.ExecutionState.IsResuming)
                return sagaStep.StepName;
            
            if (saga.ExecutionState.IsCompensating)
            {
                StepData latestToCompensate = saga.ExecutionState.History.
                    GetNextToCompensate(saga.ExecutionState.ExecutionID);

                if (latestToCompensate != null)
                    return latestToCompensate.StepName;

                return null;
            }

            return sagaAction.
                GetNextStepToExecute(sagaStep, saga.ExecutionState)?.StepName;
        }

    }
}