using TheSaga.Events;
using TheSaga.Models;

namespace TheSaga.Builders
{
    public interface ISagaBuilderHandle<TSagaData, TEvent> : ISagaBuilderThen<TSagaData>
        where TSagaData : ISagaData
        where TEvent : ISagaEvent
    {
        ISagaBuilderThen<TSagaData> HandleBy<TEventHandler>()
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;
    }
}