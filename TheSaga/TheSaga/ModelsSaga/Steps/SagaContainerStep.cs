using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models.History;
using TheSaga.ModelsSaga.Steps.Interfaces;

namespace TheSaga.ModelsSaga.Steps
{
    internal class SagaContainerStep : ISagaStep
    {
        public ISagaStep ParentStep { get; }
        public SagaSteps ChildSteps { get; private set; }

        public SagaContainerStep(
            string StepName, ISagaStep parentStep)
        {
            this.StepName = StepName;
            Async = false;
            ChildSteps = new SagaSteps();
            ParentStep = parentStep;
        }

        public bool Async { get; }
        public string StepName { get; }

        public void SetChildSteps(SagaSteps steps)
        {
            this.ChildSteps = steps;
        }

        public Task Compensate(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            return Task.CompletedTask;
        }
    }
}
