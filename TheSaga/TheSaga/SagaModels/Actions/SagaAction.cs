using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Models;
using TheSaga.SagaModels.Steps;

namespace TheSaga.SagaModels.Actions
{
    internal class SagaAction<TSagaData> : ISagaAction
        where TSagaData : ISagaData
    {
        public SagaAction()
        {
            Steps = new List<ISagaStep>();
        }

        public Type Event { get; set; }
        public string State { get; set; }
        public List<ISagaStep> Steps { get; set; }

        public void AddStep(ISagaStep step)
        {
            Steps.Add(step);
        }

        public ISagaStep FindFirstStep()
        {
            return Steps.FirstOrDefault();
        }

        public ISagaStep FindNextAfter(ISagaStep step)
        {
            bool stepFound = false;
            foreach (ISagaStep curStep in Steps)
            {
                if (stepFound)
                    return curStep;

                if (curStep == step)
                    stepFound = true;
            }

            return null;
        }

        public ISagaStep FindPrevBefore(ISagaStep step)
        {
            int stepIndex = Steps.IndexOf(step);
            if (stepIndex > 0)
                return Steps[stepIndex - 1];
            return null;
        }

        public ISagaStep FindStep(string stepName)
        {
            ISagaStep step = Steps.FirstOrDefault(s => s.StepName == stepName);

            return step;
        }
    }
}