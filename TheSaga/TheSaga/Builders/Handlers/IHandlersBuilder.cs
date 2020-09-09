using TheSaga.Events;
using TheSaga.Models;
using TheSaga.ModelsHandlers;
using TheSaga.SagaModels;
using TheSaga.States;

namespace TheSaga.Builders.Handlers
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