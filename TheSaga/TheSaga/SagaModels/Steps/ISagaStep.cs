using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.ValueObjects;

namespace TheSaga.SagaModels.Steps
{
    public interface ISagaStep
    {
        bool Async { get; }
        string StepName { get; }
        Task Compensate(IExecutionContext context, IEvent @event);
        Task Execute(IExecutionContext context, IEvent @event);
    }
}