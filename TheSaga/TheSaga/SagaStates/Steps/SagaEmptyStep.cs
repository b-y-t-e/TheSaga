using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Execution.Context;

namespace TheSaga.SagaStates.Steps
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

        public Task Run(IExecutionContext context, IEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}