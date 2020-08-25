using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSaga.States.Actions
{
    public class SagaAction<TSagaState> : ISagaAction
        where TSagaState : ISagaState
    {
        public String State { get; set; }

        public Type Event { get; set; }

        public List<ISagaStep> Steps { get; set; }

        public SagaAction()
        {
            Steps = new List<ISagaStep>();
        }

        public ISagaStep FindStep(string stepName)
        {
            ISagaStep step = Steps.
                FirstOrDefault(s => s.StepName == stepName);

            if (step == null)
                step = Steps.FirstOrDefault();

            return step;
        }

        public ISagaStep FindNextAfter(ISagaStep step)
        {
            bool stepFound = false;
            foreach(ISagaStep curStep in this.Steps )
            {
                if (stepFound)
                    return curStep;

                if (curStep == step)
                    stepFound = true;
            }
            return null;
        }
    }
}