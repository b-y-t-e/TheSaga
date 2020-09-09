using System;
using TheSaga.Models;
using TheSaga.Providers;
using TheSaga.ValueObjects;

namespace TheSaga.SagaModels.History
{
    public class StepData
    {
        public Guid ExecutionID { get; set; }
        public string StepName { get; set; }
        public string StateName { get; set; }
        public StepExecutionData ExecutionData { get; set; }
        public StepExecutionData CompensationData { get; set; }
        public StepExecutionData ResumeData { get; set; }
        public bool AsyncExecution { get; set; }
        public bool AsyncStep { get; set; }

        public void MarkStarted(SagaState state, IDateTimeProvider dateTimeProvider)
        {
            if (state.IsResuming)
                ResumeData.StartTime = dateTimeProvider.Now;
            else if (state.IsCompensating)
                CompensationData.StartTime = dateTimeProvider.Now;
            else
                ExecutionData.StartTime = dateTimeProvider.Now;
        }

        public void MarkEnded(SagaState state, IDateTimeProvider dateTimeProvider)
        {
            if (state.IsResuming)
                ResumeData.EndTime = dateTimeProvider.Now;
            else if (state.IsCompensating)
                CompensationData.EndTime = dateTimeProvider.Now;
            else
                ExecutionData.EndTime = dateTimeProvider.Now;
        }

        public void MarkSucceeded(SagaState state, IDateTimeProvider dateTimeProvider)
        {
            if (state.IsResuming)
                ResumeData.SucceedTime = dateTimeProvider.Now;
            else if (state.IsCompensating)
                CompensationData.SucceedTime = dateTimeProvider.Now;
            else
                ExecutionData.SucceedTime = dateTimeProvider.Now;
        }

        public void MarkFailed(SagaState state, IDateTimeProvider dateTimeProvider, Exception error)
        {
            if (state.IsResuming)
            {
                ResumeData.Error = error;
                ResumeData.FailTime = dateTimeProvider.Now;
            }
            else if (state.IsCompensating)
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
                CompensationData.NextStepName = stepName;
            else
                ExecutionData.NextStepName = stepName;
        }

        internal void SetEndStateName(string currentState)
        {
            if (CompensationData != null)
                CompensationData.EndStateName = currentState;
            else
                ExecutionData.EndStateName = currentState;
        }

        internal bool HasSucceeded()
        {
            return ExecutionData?.SucceedTime != null;
        }
    }
}