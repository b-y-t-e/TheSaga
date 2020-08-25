using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Events;
using TheSaga.Execution.Context;

namespace TheSaga.SagaStates.Steps
{
    public interface ISagaStep
    {
        bool Async { get; }
        string StepName { get; }

        Task Run(IExecutionContext context, IEvent @event);
    }
}