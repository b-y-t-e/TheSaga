using TheSaga.Handlers.Events;
using TheSaga.Handlers.ModelsHandlers;

namespace TheSaga.Handlers.Builders
{
    public interface IHandlersBuilder
    {
        IHandlersModel Build();

        IHandlersBuilderHandle<TEvent> When<TEvent>() where TEvent : IHandlersEvent;

        IHandlersBuilderHandle<TEvent> When<TEvent, TEventHandler>() where TEvent : IHandlersEvent
            where TEventHandler : IHandlersCompensateEventHandler<TEvent>;

        IHandlersBuilderHandle<TEvent> WhenAsync<TEvent, TEventHandler>() where TEvent : IHandlersEvent
            where TEventHandler : IHandlersCompensateEventHandler<TEvent>;
    }
}
