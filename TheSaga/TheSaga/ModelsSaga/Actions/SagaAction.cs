using System;
using TheSaga.ModelsSaga.Actions.Interfaces;
using TheSaga.ModelsSaga.Steps;

namespace TheSaga.ModelsSaga.Actions
{

    public class SagaAction : ISagaAction
    {
        public SagaAction()
        {
            ChildSteps = new SagaSteps();
        }

        public Type Event { get; set; }
        public string State { get; set; }
        public SagaSteps ChildSteps { get; set; }

    }
}
