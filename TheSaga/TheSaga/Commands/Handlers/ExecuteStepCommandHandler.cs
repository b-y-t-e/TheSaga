using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.ExecutionContext;
using TheSaga.MessageBus;
using TheSaga.Messages;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Actions;
using TheSaga.ModelsSaga.Actions.Interfaces;
using TheSaga.Models.History;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.ModelsSaga.Steps.Interfaces;
using TheSaga.Persistance;
using TheSaga.Providers;
using TheSaga.Providers.Interfaces;
using TheSaga.States;
using TheSaga.Utils;
using TheSaga.ValueObjects;
using System.Collections.Generic;

namespace TheSaga.Commands.Handlers
{
    internal class ExecuteStepCommandHandler
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ISagaPersistance sagaPersistance;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public ExecuteStepCommandHandler(
            ISagaPersistance sagaPersistance,
            IServiceProvider serviceProvider,
            IDateTimeProvider dateTimeProvider, ILogger logger)
        {
            this.sagaPersistance = sagaPersistance;
            this.serviceProvider = serviceProvider;
            this.dateTimeProvider = dateTimeProvider;
            this.logger = logger;
        }

        public async Task<ISaga> Handle(ExecuteStepCommand command)
        {
            ISaga saga = command.Saga;
            ISagaStep step = command.SagaStep;
            ISagaAction sagaAction = command.SagaAction;
            ISagaModel model = command.Model;

            StepData stepData = GetOrCreateStepData(saga, step, model);

            MiddlewaresChain middlewaresChain = Middlewares.BuildFullChain(
                serviceProvider,
                SaveSaga, ExecuteStep);

            Exception executionError = null;
            try
            {
                await Middlewares.ExecuteChain(
                    middlewaresChain,
                    saga, step, stepData);

                stepData.
                    SetSucceeded(saga.ExecutionState, dateTimeProvider);
            }
            catch (SagaStopException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.
                    LogError(ex, $"Saga: {saga.Data.ID}; Executing {(step.Async ? "async " : "")}step: {step.StepName}");

                executionError = ex;

                stepData.
                    SetFailed(saga.ExecutionState, dateTimeProvider, executionError.ToSagaStepException());
            }
            finally
            {
                middlewaresChain.
                    Clean();

                stepData.
                    SetEnded(saga.ExecutionState, dateTimeProvider);
            }

            string nextStepName = CalculateNextStepName(
                saga, step, sagaAction, stepData, executionError);

            SaveNextStep(saga, stepData, nextStepName);

            CheckIfSagaIsDeleted(saga);

            await sagaPersistance.Set(saga);

            return saga;
        }

        private static void CheckIfSagaIsDeleted(ISaga saga)
        {
            if (saga.HasError() &&
                saga.ExecutionState.CurrentState == new SagaStartState().GetStateName())
            {
                if (saga.ExecutionState.CurrentStep == null)
                    saga.ExecutionState.IsDeleted = true;
            }
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

        private string CalculateNextStepName(
            ISaga saga,
            ISagaStep sagaStep,
            ISagaAction sagaAction,
            StepData stepData,
            Exception executionError)
        {
            if (saga.ExecutionState.IsBreaked)
                return null;

            if (executionError != null)
            {
                saga.ExecutionState.IsResuming = false;
                saga.ExecutionState.IsCompensating = true;
                saga.ExecutionState.CurrentError = executionError.ToSagaStepException();
                return CalculateNextCompensationStep(saga);
            }
            else
            {
                string nextStepName = CalculateNextStep(saga, sagaAction, sagaStep, stepData);
                saga.ExecutionState.IsResuming = false;
                return nextStepName;
            }
        }

        private async Task SaveSaga(ISaga saga, ISagaStep sagaStep, StepData stepData)
        {
            await sagaPersistance.Set(saga);
        }

        private async Task ExecuteStep(ISaga saga, ISagaStep sagaStep, StepData stepData)
        {
            ISagaEvent @event = stepData.Event;
            if (@event is EmptyEvent)
                @event = null;

            Type executionContextType =
                typeof(ExecutionContext<>).ConstructGenericType(saga.Data.GetType());

            IExecutionContext context = (IExecutionContext)ActivatorUtilities.CreateInstance(serviceProvider,
                executionContextType, saga.Data, saga.ExecutionInfo, saga.ExecutionState, saga.ExecutionValues, stepData.ExecutionValues);

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
