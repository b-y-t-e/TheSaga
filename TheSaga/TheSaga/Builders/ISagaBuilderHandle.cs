using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Builders
{
    public interface ISagaBuilderHandle<TSagaData, TEvent> : ISagaBuilderThen<TSagaData>
        where TSagaData : ISagaData
        where TEvent : ISagaEvent
    {
        ISagaBuilderThen<TSagaData> HandleBy<TEventHandler>()
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;
        ISagaBuilderThen<TSagaData> HandleByAsync<TEventHandler>()
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;

        ISagaBuilderThen<TSagaData> HandleBy<TEventHandler>(string stepName)
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;
        ISagaBuilderThen<TSagaData> HandleByAsync<TEventHandler>(string stepName)
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;
    }
}
