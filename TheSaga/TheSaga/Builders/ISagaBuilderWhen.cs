using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Builders
{
    public interface ISagaBuilderWhen<TSagaData> : ISagaBuilder<TSagaData>
        where TSagaData : ISagaData
    {
        ISagaBuilderThen<TSagaData, TEvent> When<TEvent>() where TEvent : ISagaEvent;

        ISagaBuilderThen<TSagaData, TEvent> When<TEvent, TEventHandler>() where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;

        ISagaBuilderThen<TSagaData, TEvent> WhenAsync<TEvent, TEventHandler>() where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;
    }
}
