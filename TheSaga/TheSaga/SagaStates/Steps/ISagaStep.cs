using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Interfaces;

namespace TheSaga.States.Actions
{
    public interface ISagaStep
    {
        bool Async { get; }
        string StepName { get; }

        Task Execute(IExecutionContext context, IEvent @event);
    }
}