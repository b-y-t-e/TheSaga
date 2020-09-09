using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Exceptions;
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
                if (model.ResumePolicy == ESagaResumePolicy.DoCurrentStepCompensation)
                {
                    saga.State.IsResuming = true;
                }
                else if (model.ResumePolicy == ESagaResumePolicy.DoFullCompensation)
                {
                    throw new SagaCompensateAllOnResumeException();
                }
                else
                {
                    saga.State.IsResuming = false;
                }
            }

            StepData stepData = null;
            if (saga.State.IsResuming)
            {
                stepData = currentExecutionData;

                stepData.ResumeData = new StepExecutionData
                {
                    EndStateName = saga.State.CurrentState,
                    StepType = step.GetType()
                };
            }
            else if (saga.State.IsCompensating)
            {
                stepData = saga.State.History.
                    GetLatestCompensationByStepName(
                        saga.State.ExecutionID,
                        saga.State.CurrentStep);

                stepData.CompensationData = new StepExecutionData()
                {
                    StepType = step.GetType()
                };
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
                        EndStateName = saga.State.CurrentState,
                        StepType = step.GetType()
                    }
                };
                saga.State.History.Add(stepData);
            }

            return stepData;
        }
    }
}