using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.SagaModels.Actions;
using TheSaga.ValueObjects;

namespace TheSaga.SagaModels.Steps
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

        public Task Compensate(IExecutionContext context, ISagaEvent @event)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext context, ISagaEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}