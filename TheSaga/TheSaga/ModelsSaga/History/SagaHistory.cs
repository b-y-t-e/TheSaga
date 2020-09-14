using System.Collections.Generic;
using System.Linq;
using TheSaga.Exceptions;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.ModelsSaga.Steps.Interfaces;
using TheSaga.ValueObjects;

namespace TheSaga.ModelsSaga.History
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

        public StepData GetLatestByStepName(
            string stepName)
        {
            StepData executionData = this.LastOrDefault(s =>
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
            ISagaStep step,
            ISagaModel model)
        {
            StepData currentExecutionData = saga.ExecutionState.History.
                LastOrDefault();

            if (!saga.ExecutionState.IsCompensating &&
                currentExecutionData != null &&
                currentExecutionData.CompensationData == null &&
                currentExecutionData.ExecutionData != null &&
                currentExecutionData.ExecutionID == saga.ExecutionState.ExecutionID &&
                currentExecutionData.StepName == saga.ExecutionState.CurrentStep &&
                currentExecutionData.ResumeData?.EndTime == null)
            {
                if (model.ResumePolicy == ESagaResumePolicy.DoCurrentStepCompensation)
                {
                    saga.ExecutionState.IsResuming = true;
                }
                else if (model.ResumePolicy == ESagaResumePolicy.DoFullCompensation)
                {
                    throw new SagaCompensateAllOnResumeException();
                }
                else
                {
                    saga.ExecutionState.IsResuming = false;
                }
            }

            StepData stepData = null;
            if (saga.ExecutionState.IsResuming)
            {
                stepData = currentExecutionData;

                stepData.ResumeData = new StepExecutionData
                {
                    EndStateName = saga.ExecutionState.CurrentState,
                    StepType = step.GetType()
                };
            }
            else if (saga.ExecutionState.IsCompensating)
            {
                stepData = saga.ExecutionState.History.
                    GetLatestCompensationByStepName(
                        saga.ExecutionState.ExecutionID,
                        saga.ExecutionState.CurrentStep);

                stepData.CompensationData = new StepExecutionData()
                {
                    StepType = step.GetType()
                };
            }
            else
            {
                stepData = new StepData
                {
                    ExecutionID = saga.ExecutionState.ExecutionID,
                    AsyncExecution = saga.ExecutionState.AsyncExecution,
                    AsyncStep = step.Async,
                    StateName = saga.ExecutionState.CurrentState,
                    StepName = saga.ExecutionState.CurrentStep,
                    Event = saga.ExecutionState.CurrentEvent,
                    ExecutionValues = new StepExecutionValues(),
                    ExecutionData = new StepExecutionData
                    {
                        EndStateName = saga.ExecutionState.CurrentState,
                        StepType = step.GetType()
                    }
                };
                saga.ExecutionState.History.Add(stepData);
            }

            return stepData;
        }
    }
}
