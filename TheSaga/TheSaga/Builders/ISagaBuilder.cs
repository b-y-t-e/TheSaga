using TheSaga.Events;
using TheSaga.Models;
using TheSaga.SagaModels;
using TheSaga.States;

namespace TheSaga.Builders
{
    public interface ISagaBuilder<TSagaData> where TSagaData : ISagaData
    {
        ISagaModel<TSagaData> Build();

        ISagaBuilderWhen<TSagaData> During<TState>() where TState : IState;

        ISagaBuilderThen<TSagaData> Start<TEvent>() where TEvent : IEvent;

        ISagaBuilderThen<TSagaData> Start<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaData, TEvent>;

        ISagaBuilderThen<TSagaData> Start<TEvent>(string stepName) where TEvent : IEvent;

        ISagaBuilderThen<TSagaData> Start<TEvent, TEventHandler>(string stepName) 
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>;

        ISagaBuilderThen<TSagaData> StartAsync<TEvent, TEventHandler>() 
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>;

        ISagaBuilderThen<TSagaData> StartAsync<TEvent, TEventHandler>(string stepName) 
            where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaData, TEvent>;
    }
}