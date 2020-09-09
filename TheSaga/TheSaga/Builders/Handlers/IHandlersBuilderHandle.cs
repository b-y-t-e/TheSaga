using TheSaga.Events;
using TheSaga.Models;

namespace TheSaga.Builders.Handlers
{
    public interface IHandlersBuilderHandle<TEvent> : IHandlersBuilderThen
        where TEvent : IHandlersEvent
    {
        IHandlersBuilderHandle<TEvent> HandleBy<TEventHandler>()
            where TEventHandler : IHandlersCompensateEventHandler<TEvent>;

    }
}