using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Handlers.ExecutionContext;

namespace TheSaga.Handlers.Events
{
    public interface IHandlersCompensateEventHandler<TEvent> : ISagaEventHandler
        where TEvent : IHandlersEvent
    {
        Task Compensate(IHandlersExecutionContext context, TEvent @event);

        Task Execute(IHandlersExecutionContext context, TEvent @event);
    }
}