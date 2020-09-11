using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.ModelsSaga.History;
using TheSaga.ModelsSaga.Steps.Interfaces;

namespace TheSaga.ModelsSaga.Steps
{
    internal class SagaEmptyStep : ISagaStep
    {
        public ISagaStep ParentStep { get; }
        public SagaSteps ChildSteps { get; }

        public SagaEmptyStep(
            string StepName, ISagaStep parentStep)
        {
            this.StepName = StepName;
            Async = false;
            ChildSteps = new SagaSteps();
            ParentStep = parentStep;
        }

        public bool Async { get; }
        public string StepName { get; }

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
