using TheSaga.Events;
using TheSaga.Models;

namespace TheSaga.Builders
{
    public interface ISagaBuilderWhen<TSagaData> : ISagaBuilder<TSagaData>
        where TSagaData : ISagaData
    {
        ISagaBuilderHandle<TSagaData, TEvent> When<TEvent>() where TEvent : IEvent;

        ISagaBuilderHandle<TSagaData, TEvent> When<TEvent, TEventHandler>() where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>;

        ISagaBuilderHandle<TSagaData, TEvent> WhenAsync<TEvent, TEventHandler>() where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>;
    }
}