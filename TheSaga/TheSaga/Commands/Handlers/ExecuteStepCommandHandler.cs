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
            ISagaModel model = command.Model;

            StepData stepData = GetOrCreateStepData(saga, sagaStep, model);

            await sagaPersistance.Set(saga);

            ISagaEvent @event = stepData.Event;
            if (@event is EmptyEvent)
                @event = null;

            Exception executionError = null;
            try
            {
                await ExecuteStep(saga, sagaStep, stepData, @event);

                stepData.
                    SetSucceeded(saga.ExecutionState, dateTimeProvider);
            }
            catch (SagaStopException)
            {
                return null;
            }
            catch (Exception ex)
            {
                executionError = ex;

                stepData.
                    SetFailed(saga.ExecutionState, dateTimeProvider, executionError.ToSagaStepException());
            }
            finally
            {
                stepData.
                    SetEnded(saga.ExecutionState, dateTimeProvider);
            }

            string nextStepName = CalculateNextStepName(
                saga, sagaStep, sagaAction, stepData, executionError);
            
            SaveNextStep(saga, stepData, nextStepName);

            await sagaPersistance.Set(saga);

            return saga;
        }

        private void SaveNextStep(ISaga saga, StepData stepData, string nextStepName)
        {
            stepData.
                SetNextStepName(nextStepName).
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
        }

        private string CalculateNextStepName(ISaga saga, ISagaStep sagaStep, ISagaAction sagaAction, StepData stepData, Exception executionError)
        {
            string nextStepName = null;
            if (executionError != null)
            {
                saga.ExecutionState.IsResuming = false;
                saga.ExecutionState.IsCompensating = true;
                saga.ExecutionState.CurrentError = executionError.ToSagaStepException();
                nextStepName = CalculateNextCompensationStep(saga);
            }
            else
            {
                nextStepName = CalculateNextStep(saga, sagaAction, sagaStep, stepData);
                saga.ExecutionState.IsResuming = false;
            }

            return nextStepName;
        }

        private async Task ExecuteStep(ISaga saga, ISagaStep sagaStep, StepData stepData, ISagaEvent @event)
        {
            Type executionContextType =
                                typeof(ExecutionContext<>).ConstructGenericType(saga.Data.GetType());

            IExecutionContext context = (IExecutionContext)ActivatorUtilities.CreateInstance(serviceProvider,
                executionContextType, saga.Data, saga.ExecutionInfo, saga.ExecutionState, saga.ExecutionValues);

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
        }

        private StepData GetOrCreateStepData(ISaga saga, ISagaStep sagaStep, ISagaModel model)
        {
            saga.ExecutionState.CurrentStep = sagaStep.StepName;
            saga.ExecutionInfo.Modified = dateTimeProvider.Now;

            StepData stepData = saga.ExecutionState.History.
                Create(saga, sagaStep, model).
                SetStarted(saga.ExecutionState, dateTimeProvider);

            saga.ExecutionState.CurrentEvent = new EmptyEvent();
            return stepData;
        }

        private string CalculateNextStep(ISaga saga, ISagaAction sagaAction, ISagaStep sagaStep, IStepData stepData = null)
        {
            if (saga.ExecutionState.IsResuming)
                return sagaStep.StepName;
            
            if (saga.ExecutionState.IsCompensating)            
                return CalculateNextCompensationStep(saga);            

            return sagaAction.
                GetNextStepToExecute(sagaStep, saga.ExecutionState)?.StepName;
        }

        private string CalculateNextCompensationStep(ISaga saga)
        {
            if (saga.ExecutionState.IsCompensating)
            {
                StepData latestToCompensate = saga.ExecutionState.History.
                    GetNextToCompensate(saga.ExecutionState.ExecutionID);

                if (latestToCompensate != null)
                    return latestToCompensate.StepName;

                return null;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}