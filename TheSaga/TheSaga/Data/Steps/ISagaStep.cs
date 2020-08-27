using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Execution.Context;

namespace TheSaga.SagaStates.Steps
{
    public interface ISagaStep
    {
        bool Async { get; }
        string StepName { get; }
        Task Compensate(IExecutionContext context, IEvent @event);
        Task Execute(IExecutionContext context, IEvent @event);
    }
}