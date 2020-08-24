using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Interfaces;

namespace TheSaga.States.Actions
{
    public interface ISagaStep
    {
        bool Async { get; }
        string StepName { get; }

        Task Execute(IInstanceContext context, IEvent @event);
    }
}