using System.Collections.Generic;
using System.Linq;
using TheSaga.Providers;
using TheSaga.ValueObjects;

namespace TheSaga.Models.Steps
{
    public class SagaHistory : List<StepData>
    {
        public StepData GetLatestToCompensateByStepName(
            ExecutionID executionID,
            string stepName)
        {
            StepData executionData = this.
                LastOrDefault(s =>
                    s.ExecutionID == executionID &&
                    s.CompensationData == null &&
                    s.StepName == stepName);

            return executionData;
        }

        public StepData GetLatestByStepName(
            ExecutionID executionID,
            string stepName)
        {
            StepData executionData = this.
                LastOrDefault(s =>
                    s.ExecutionID == executionID &&
                    s.StepName == stepName);

            return executionData;
        }

        public StepData GetLatestToCompensate(
            ExecutionID executionID)
        {
            StepData executionData = this.
                LastOrDefault(s =>
                    s.ExecutionID == executionID &&
                    s.CompensationData == null);

            return executionData;
        }

        public StepData PrepareExecutionData(
            ISaga saga,
            AsyncExecution isExecutionAsync,
            IDateTimeProvider dateTimeProvider)
        {
            StepData stepData = null;

            if (saga.State.IsCompensating)
            {
                stepData = saga.State.History.
                    GetLatestToCompensateByStepName(saga.State.ExecutionID, saga.State.CurrentStep);

                stepData.CompensationData = new StepExecutionData();
            }
            else
            {
                stepData = new StepData()
                {
                    ExecutionID = saga.State.ExecutionID,
                    Async = isExecutionAsync,
                    StateName = saga.State.CurrentState,
                    StepName = saga.State.CurrentStep,
                    ExecutionData = new StepExecutionData()
                    {
                        StartTime = dateTimeProvider.Now,
                        EndStateName = saga.State.CurrentState
                    }
                };
                saga.State.History.Add(stepData);
            }

            stepData.
                Started(dateTimeProvider);

            return stepData;
        }
    }
}