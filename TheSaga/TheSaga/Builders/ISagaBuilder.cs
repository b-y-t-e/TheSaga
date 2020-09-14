using System;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.States;
using TheSaga.States.Interfaces;

namespace TheSaga.Builders
{
    public interface ISagaBuilder<TSagaData> where TSagaData : ISagaData
    {
        ISagaModel Build();

        ISagaBuilderWhen<TSagaData> During<TState>() where TState : ISagaState;

        ISagaBuilder<TSagaData> Name(string name);
        ISagaBuilder<TSagaData> Settings(Action<ISagaSettingsBuilder> settingsBuilder);
        ISagaBuilderThen<TSagaData, TEvent> Start<TEvent>()
            where TEvent : ISagaEvent;

        ISagaBuilderThen<TSagaData, TEvent> Start<TEvent, TEventHandler>() where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;

        ISagaBuilderThen<TSagaData, TEvent> Start<TEvent>(string stepName)
            where TEvent : ISagaEvent;

        ISagaBuilderThen<TSagaData, TEvent> Start<TEvent, TEventHandler>(string stepName)
            where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;

        ISagaBuilderThen<TSagaData, TEvent> StartAsync<TEvent, TEventHandler>()
            where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;

        ISagaBuilderThen<TSagaData, TEvent> StartAsync<TEvent, TEventHandler>(string stepName)
            where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;
    }
}
