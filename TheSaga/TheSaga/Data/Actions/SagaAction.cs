using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.SagaStates.Steps;

namespace TheSaga.SagaStates.Actions
{
    internal class SagaAction<TSagaData> : ISagaAction
        where TSagaData : ISagaData
    {
        public SagaAction()
        {
            Steps = new List<ISagaStep>();
        }

        public Type Event { get; set; }
        public String State { get; set; }
        public List<ISagaStep> Steps { get; set; }

        public ISagaStep FindNextAfter(ISagaStep step)
        {
            bool stepFound = false;
            foreach (ISagaStep curStep in this.Steps)
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
            int stepIndex = this.Steps.IndexOf(step);
            if (stepIndex > 0)
                return this.Steps[stepIndex - 1];
            return null;
        }

        public ISagaStep FindStep(string stepName)
        {
            ISagaStep step = Steps.
                FirstOrDefault(s => s.StepName == stepName);

            return step;
        }
    }
}