using System;
using TheSaga.Providers;
using TheSaga.ValueObjects;

namespace TheSaga.SagaModels.History
{
    public class StepData
    {
        public Guid ExecutionID { get; set; }
        public String StepName { get; set; }
        public String StateName { get; set; }
        public StepExecutionData ExecutionData { get; set; }
        public StepExecutionData CompensationData { get; set; }
        public AsyncExecution Async { get; set; }

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
}