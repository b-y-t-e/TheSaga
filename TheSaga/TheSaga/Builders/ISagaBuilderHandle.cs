using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Builders
{
    public interface ISagaBuilderHandle<TSagaData, TEvent> : ISagaBuilderThen<TSagaData>
        where TSagaData : ISagaData
        where TEvent : ISagaEvent
    {
        ISagaBuilderHandle<TSagaData, TEvent> HandleBy<TEventHandler>()
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;
        ISagaBuilderHandle<TSagaData, TEvent> HandleByAsync<TEventHandler>()
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;

        ISagaBuilderHandle<TSagaData, TEvent> HandleBy<TEventHandler>(string stepName)
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;
        ISagaBuilderHandle<TSagaData, TEvent> HandleByAsync<TEventHandler>(string stepName)
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;
    }
}
