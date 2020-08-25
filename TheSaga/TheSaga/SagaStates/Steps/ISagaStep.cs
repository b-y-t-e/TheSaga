using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Execution.Context;

namespace TheSaga.SagaStates.Steps
{
    public interface ISagaStep
    {
        bool Async { get; }
        string StepName { get; }

        Task Execute(IExecutionContext context, IEvent @event);
        Task Compensate(IExecutionContext context, IEvent @event);
    }
}