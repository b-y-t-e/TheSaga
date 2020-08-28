using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaEmptyStep : ISagaStep
    {
        public SagaEmptyStep(
            String StepName)
        {
            this.StepName = StepName;
            this.Async = false;
        }

        public bool Async { get; }
        public String StepName { get; }

        public Task Compensate(IExecutionContext context, IEvent @event)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext context, IEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}