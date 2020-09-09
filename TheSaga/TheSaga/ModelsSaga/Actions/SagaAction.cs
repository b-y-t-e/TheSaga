using System;
using System.Linq;
using TheSaga.Models;
using TheSaga.SagaModels.Steps;

namespace TheSaga.SagaModels.Actions
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