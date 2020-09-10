﻿using System;
using TheSaga.Builders.Saga;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.SagaModels;
using TheSaga.States;

namespace TheSaga.Builders
{
    public interface ISagaBuilder<TSagaData> where TSagaData : ISagaData
    {
        ISagaModel Build();

        ISagaBuilderWhen<TSagaData> During<TState>() where TState : ISagaState;

        ISagaBuilder<TSagaData> Name(string name);
        ISagaBuilder<TSagaData> Settings(Action<ISagaSettingsBuilder> settingsBuilder);
        ISagaBuilderHandle<TSagaData, TEvent> Start<TEvent>() 
            where TEvent : ISagaEvent;

        ISagaBuilderThen<TSagaData> Start<TEvent, TEventHandler>() where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;

        ISagaBuilderHandle<TSagaData, TEvent> Start<TEvent>(string stepName) 
            where TEvent : ISagaEvent;

        ISagaBuilderThen<TSagaData> Start<TEvent, TEventHandler>(string stepName)
            where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;

        ISagaBuilderThen<TSagaData> StartAsync<TEvent, TEventHandler>()
            where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;

        ISagaBuilderThen<TSagaData> StartAsync<TEvent, TEventHandler>(string stepName)
            where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>;
    }
}