using System;
using System.Collections.Generic;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Providers.Interfaces;
using TheSaga.Utils;

namespace TheSaga.Models.History
{
    public class StepData : IStepData
    {
        public Guid ID { get; set; }
        public Guid ExecutionID { get; set; }
        public string StateName { get; set; }
        public string StepName { get; set; }
        public string EndStateName { get; set; }
        public string NextStepName { get; set; }
        public bool AsyncExecution { get; set; }
        public bool AsyncStep { get; set; }
        public ISagaEvent Event { get; set; }
        public StepExecutionValues ExecutionValues { get; set; }
        public StepExecutionData ExecutionData { get; set; }
        public StepExecutionData CompensationData { get; set; }
        public StepExecutionData ResumeData { get; set; }

        //public StepRetry Retry { get; set; }

        public StepData()
        {
            ID = Guid.NewGuid();
        }
    }

    public static class StepDataExtensions
    {
        static public StepData SetStarted(this StepData data, SagaExecutionState state, IDateTimeProvider dateTimeProvider)
        {
            if (state.IsResuming)
                data.ResumeData.StartTime = dateTimeProvider.Now;
            else if (state.IsCompensating)
                data.CompensationData.StartTime = dateTimeProvider.Now;
            else
                data.ExecutionData.StartTime = dateTimeProvider.Now;
            return data;
        }

        static public StepData SetEnded(this StepData data, SagaExecutionState state, IDateTimeProvider dateTimeProvider)
        {
            if (state.IsResuming)
                data.ResumeData.EndTime = dateTimeProvider.Now;
            else if (state.IsCompensating)
                data.CompensationData.EndTime = dateTimeProvider.Now;
            else
                data.ExecutionData.EndTime = dateTimeProvider.Now;
            return data;
        }

        static public StepData SetSucceeded(this StepData data, SagaExecutionState state, IDateTimeProvider dateTimeProvider)
        {
            if (state.IsResuming)
                data.ResumeData.SucceedTime = dateTimeProvider.Now;
            else if (state.IsCompensating)
                data.CompensationData.SucceedTime = dateTimeProvider.Now;
            else
                data.ExecutionData.SucceedTime = dateTimeProvider.Now;
            return data;
        }

        static public StepData SetFailed(this StepData data, SagaExecutionState state, IDateTimeProvider dateTimeProvider, Exception error)
        {
            if (state.IsResuming)
            {
                data.ResumeData.Error = error.ToSagaStepException();
                data.ResumeData.FailTime = dateTimeProvider.Now;
            }
            else if (state.IsCompensating)
            {
                data.CompensationData.Error = error.ToSagaStepException();
                data.CompensationData.FailTime = dateTimeProvider.Now;
            }
            else
            {
                data.ExecutionData.Error = error.ToSagaStepException();
                data.ExecutionData.FailTime = dateTimeProvider.Now;
            }
            return data;
        }

        static internal StepData SetNextStepName(this StepData data, string stepName)
        {
            data.NextStepName = stepName;
            return data;
        }

        static internal StepData SetEndStateName(this StepData data, string currentState)
        {
            data.EndStateName = currentState;
            return data;
        }

        public static bool HasSucceeded(this StepData data)
        {
            return data.ExecutionData?.SucceedTime != null;
        }
    }

    public class StepRetry
    {
        public StepRetryDelayCount DelayCount { get; set; }
        public StepRetryDelayTime DelayTime { get; set; }
        public StepRetry()
        {

        }
    }

    public class StepRetryDelayTime
    {
        public List<TimeSpan> Delays { get; set; }

        public StepRetryDelayTime()
        {
            Delays = new List<TimeSpan>();
        }
    }

    public class StepRetryDelayCount
    {
        public int Count { get; set; }
    }
}
