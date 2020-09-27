using System;
using TheSaga.Providers.Interfaces;
using TheSaga.Utils;

namespace TheSaga.Models.History
{
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

        internal static bool HasSucceeded(this StepData data)
        {
            return data.ExecutionData?.SucceedTime != null;
        }
    }
}
