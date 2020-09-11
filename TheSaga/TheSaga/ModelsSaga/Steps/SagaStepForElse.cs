using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.History;
using TheSaga.ModelsSaga.Steps.Interfaces;

namespace TheSaga.ModelsSaga.Steps
{
    internal class SagaStepForElse<TSagaData> : ISagaStep, ISagaStepElse
        where TSagaData : ISagaData
    {
        public ISagaStep ParentStep { get; }
        public SagaSteps ChildSteps { get; private set; }

        public SagaStepForElse(string StepName, ISagaStep parentStep)
        {
            this.StepName = StepName;
            this.Async = false;
            this.ChildSteps = new SagaSteps();
            this.ParentStep = parentStep;
        }

        public bool Async { get; }
        public string StepName { get; }

        public void SetChildSteps(SagaSteps steps)
        {
            this.ChildSteps = steps;
        }

        public async Task Compensate(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {

        }

        public async Task Execute(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {

        }
    }
}
