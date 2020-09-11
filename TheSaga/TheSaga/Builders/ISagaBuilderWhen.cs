using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Builders
{
    public interface ISagaBuilderWhen<TSagaData> : ISagaBuilder<TSagaData>
        where TSagaData : ISagaData
    {
        ISagaBuilderHandle<TSagaData, TEvent> When<TEvent>() where TEvent : ISagaEvent;

        ISagaBuilderHandle<TSagaData, TEvent> When<TEvent, TEventHandler>() where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;

        ISagaBuilderHandle<TSagaData, TEvent> WhenAsync<TEvent, TEventHandler>() where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;
    }
}
