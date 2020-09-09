using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Models;
using TheSaga.Providers;
using TheSaga.SagaModels.Steps;
using TheSaga.ValueObjects;

namespace TheSaga.SagaModels.History
{
    public class SagaHistory : List<StepData>
    {
        public StepData GetLatestCompensationByStepName(
            ExecutionID executionID,
            string stepName)
        {
            StepData executionData = this.LastOrDefault(s =>
                s.ExecutionID == executionID &&
                s.CompensationData == null &&
                s.StepName == stepName);

            return executionData;
        }

        public StepData GetLatestByStepName(
            ExecutionID executionID,
            string stepName)
        {
            StepData executionData = this.LastOrDefault(s =>
                s.ExecutionID == executionID &&
                s.StepName == stepName);

            return executionData;
        }

        public StepData GetNextToCompensate(
            ExecutionID executionID)
        {
            StepData executionData = this.LastOrDefault(s =>
                s.ExecutionID == executionID &&
                s.CompensationData == null);

            return executionData;
        }

        public StepData Create(
            ISaga saga,
            ISagaStep step)
        {
            StepData currentExecutionData = saga.State.History.
                LastOrDefault();

            if (!saga.State.IsCompensating &&
                currentExecutionData != null &&
                currentExecutionData.CompensationData == null &&
                currentExecutionData.ExecutionData != null &&
                currentExecutionData.ExecutionID == saga.State.ExecutionID &&
                currentExecutionData.StepName == saga.State.CurrentStep &&
                currentExecutionData.ResumeData?.EndTime == null)
            {
                saga.State.IsResuming = true;
            }

            StepData stepData = null;
            if (saga.State.IsResuming)
            {
                stepData = currentExecutionData;

                stepData.ResumeData = new StepExecutionData
                {
                    EndStateName = saga.State.CurrentState
                };
            }
            else if (saga.State.IsCompensating)
            {
                stepData = saga.State.History.
                    GetLatestCompensationByStepName(
                        saga.State.ExecutionID,
                        saga.State.CurrentStep);

                stepData.CompensationData = new StepExecutionData();
            }
            else
            {
                stepData = new StepData
                {
                    ExecutionID = saga.State.ExecutionID,
                    AsyncExecution = saga.State.AsyncExecution,
                    AsyncStep = step.Async,
                    StateName = saga.State.CurrentState,
                    StepName = saga.State.CurrentStep,
                    ExecutionData = new StepExecutionData
                    {
                        EndStateName = saga.State.CurrentState
                    }
                };
                saga.State.History.Add(stepData);
            }

            return stepData;
        }
    }
}