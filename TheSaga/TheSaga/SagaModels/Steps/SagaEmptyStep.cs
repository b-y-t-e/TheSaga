using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.ValueObjects;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaEmptyStep : ISagaStep
    {
        public SagaEmptyStep(
            string StepName)
        {
            this.StepName = StepName;
            Async = false;
        }

        public bool Async { get; }
        public string StepName { get; }

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