using System;
using System.Threading.Tasks;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Actions;
using TheSaga.ModelsSaga.Actions.Interfaces;
using TheSaga.Models.History;
using TheSaga.ModelsSaga.Steps.Interfaces;
using TheSaga.Persistance;
using TheSaga.Providers.Interfaces;
using TheSaga.States;
using TheSaga.Utils;

namespace TheSaga.Commands.Handlers
{
    public class CalculateNextStepHandler
    {
        private IDateTimeProvider dateTimeProvider;
        private ISagaPersistance sagaPersistance;

        public CalculateNextStepHandler(IDateTimeProvider dateTimeProvider, ISagaPersistance sagaPersistance)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.sagaPersistance = sagaPersistance;
        }

        public async Task CalculateNextStep(ISaga saga, ISagaStep step, ISagaAction sagaAction, StepData stepData, Exception executionError)
        {
            string nextStepName = CalculateNextStepName(
                saga, step, sagaAction, stepData, executionError);

            SaveNextStep(saga, stepData, nextStepName);

            CheckIfSagaIsDeleted(saga);

            await sagaPersistance.Set(saga);
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
        private static void CheckIfSagaIsDeleted(ISaga saga)
        {
            if (saga.HasError() &&
                saga.ExecutionState.CurrentState == new SagaStartState().GetStateName())
            {
                if (saga.ExecutionState.CurrentStep == null)
                    saga.ExecutionState.IsDeleted = true;
            }
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

    }
}
