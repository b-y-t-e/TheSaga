using TheSaga.Handlers.Events;

namespace TheSaga.Handlers.Builders
{
    public interface IHandlersBuilderHandle<TEvent> : IHandlersBuilderThen
        where TEvent : IHandlersEvent
    {
        IHandlersBuilderHandle<TEvent> HandleBy<TEventHandler>()
            where TEventHandler : IHandlersCompensateEventHandler<TEvent>;

        IHandlersBuilderHandle<TEvent> HandleByAsync<TEventHandler>()
            where TEventHandler : IHandlersCompensateEventHandler<TEvent>;

    }
}
