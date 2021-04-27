using System.Collections.Generic;
using System.Linq;
using TheSaga.Exceptions;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.ModelsSaga.Steps.Interfaces;
using TheSaga.ValueObjects;

namespace TheSaga.Models.History
{
    public class SagaHistory : List<StepData>
    {
        public StepData GetLatestCompensationByStepName(
            ExecutionID executionID,
            string stepName)
        {
            StepData executionData = this.LastOrDefault(s =>
                s.ExecutionID == executionID &&
                (s.CompensationData == null || s.CompensationData.EndTime == null) &&
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

        public StepData GetLastWithStepName(
            ExecutionID executionID,
            string stepName)
        {
            StepData executionData = this.LastOrDefault();

            if (executionData != null &&
                executionData.ExecutionID == executionID &&
                executionData.StepName == stepName)
                return executionData;

            return null;
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
            StepData stepData = saga.ExecutionState.History.
                GetLastWithStepName(saga.ExecutionState.ExecutionID, saga.ExecutionState.CurrentStep);

            if (!saga.ExecutionState.IsCompensating &&
                stepData != null &&
                stepData.CompensationData == null &&
                stepData.ExecutionData != null &&
                stepData.ResumeData?.EndTime == null)
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
            else
            {
                //stepData = null;
            }

            if (saga.ExecutionState.IsResuming)
            {
                stepData.ResumeData = new StepExecutionData
                {

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

                };
            }
            else
            {
                if (stepData == null)
                {
                    stepData = new StepData();
                    stepData.ExecutionValues = new StepExecutionValues();
                    stepData.ExecutionID = saga.ExecutionState.ExecutionID;
                    stepData.AsyncExecution = saga.ExecutionState.AsyncExecution;
                    stepData.AsyncStep = step.Async;
                    stepData.StateName = saga.ExecutionState.CurrentState;
                    stepData.StepName = saga.ExecutionState.CurrentStep;
                    stepData.Event = saga.ExecutionState.CurrentEvent;
                    saga.ExecutionState.History.Add(stepData);
                }
                else
                {

                }

                stepData.ExecutionData = new StepExecutionData
                {
                };
            }

            return stepData;
        }
    }
}
