using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Execution.Actions;
using TheSaga.Providers;

namespace TheSaga.SagaStates
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
            IsExecutionAsync isExecutionAsync,
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

    public class StepData
    {
        public Guid ExecutionID { get; set; }
        public String StepName { get; set; }
        public String StateName { get; set; }
        public StepExecutionData ExecutionData { get; set; }
        public StepExecutionData CompensationData { get; set; }
        public IsExecutionAsync Async { get; set; }

        public void Started(IDateTimeProvider dateTimeProvider)
        {
            if (CompensationData != null)
            {
                CompensationData.StartTime = dateTimeProvider.Now;
            }
            else
            {
                ExecutionData.StartTime = dateTimeProvider.Now;
            }
        }
        public void Ended(IDateTimeProvider dateTimeProvider)
        {
            if (CompensationData != null)
            {
                CompensationData.EndTime = dateTimeProvider.Now;
            }
            else
            {
                ExecutionData.EndTime = dateTimeProvider.Now;
            }
        }
        public void Succeeded(IDateTimeProvider dateTimeProvider)
        {
            if (CompensationData != null)
            {
                CompensationData.SucceedTime = dateTimeProvider.Now;
            }
            else
            {
                ExecutionData.SucceedTime = dateTimeProvider.Now;
            }
        }
        public void Failed(IDateTimeProvider dateTimeProvider, Exception error)
        {
            if (CompensationData != null)
            {
                CompensationData.Error = error;
                CompensationData.FailTime = dateTimeProvider.Now;
            }
            else
            {
                ExecutionData.Error = error;
                ExecutionData.FailTime = dateTimeProvider.Now;
            }
        }

        internal void SetNextStepName(string stepName)
        {
            if (CompensationData != null)
            {
                CompensationData.NextStepName = stepName;
            }
            else
            {
                ExecutionData.NextStepName = stepName;
            }
        }

        internal void SetEndStateName(string currentState)
        {
            if (CompensationData != null)
            {
                CompensationData.EndStateName = currentState;
            }
            else
            {
                ExecutionData.EndStateName = currentState;
            }
        }

        internal bool HasSucceeded()
        {
            return ExecutionData?.SucceedTime != null;
        }
    }

    public class StepExecutionData
    {
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? SucceedTime { get; set; }
        public DateTime? FailTime { get; set; }
        public Exception Error { get; set; }
        public string EndStateName { get; set; }
        public string NextStepName { get; set; }
    }
}