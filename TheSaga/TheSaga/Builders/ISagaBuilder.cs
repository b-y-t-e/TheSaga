using System;
using TheSaga.Activities;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Steps;
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

        ISagaBuilderThen<TSagaData> Start<TEvent, TEventHandler>(string stepName) where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaData, TEvent>;

        ISagaBuilderThen<TSagaData> StartAsync<TEvent, TEventHandler>() where TEvent : IEvent
                           where TEventHandler : IEventHandler<TSagaData, TEvent>;

        ISagaBuilderThen<TSagaData> StartAsync<TEvent, TEventHandler>(string stepName) where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaData, TEvent>;
    }


    public interface ISagaBuilderHandle<TSagaData> : ISagaBuilderThen<TSagaData>
        where TSagaData : ISagaData
    {
        ISagaBuilderThen<TSagaData> HandleBy<TEventHandler>()
            where TEventHandler : IEventHandler;
    }

    public interface ISagaBuilderWhen<TSagaData> : ISagaBuilder<TSagaData>
        where TSagaData : ISagaData
    {

        ISagaBuilderHandle<TSagaData> When<TEvent>() where TEvent : IEvent;

        ISagaBuilderHandle<TSagaData> When<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaData, TEvent>;

        ISagaBuilderHandle<TSagaData> WhenAsync<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaData, TEvent>;
    }

    public interface ISagaBuilderThen<TSagaData> : ISagaBuilderWhen<TSagaData>
            where TSagaData : ISagaData
    {
        ISagaBuilderThen<TSagaData> After(TimeSpan time);

        ISagaModel<TSagaData> Build();

        ISagaBuilder<TSagaData> Finish();

        ISagaBuilderThen<TSagaData> Then(ThenActionDelegate<TSagaData> action);

        ISagaBuilderThen<TSagaData> Then(string stepName, ThenActionDelegate<TSagaData> action);

        ISagaBuilderThen<TSagaData> Then(ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation);

        ISagaBuilderThen<TSagaData> Then(string stepName, ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation);

        ISagaBuilderThen<TSagaData> Then<TSagaActivity>() where TSagaActivity : ISagaActivity<TSagaData>;

        ISagaBuilderThen<TSagaData> Then<TSagaActivity>(string stepName) where TSagaActivity : ISagaActivity<TSagaData>;

        ISagaBuilderThen<TSagaData> ThenAsync(ThenActionDelegate<TSagaData> action);

        ISagaBuilderThen<TSagaData> ThenAsync(string stepName, ThenActionDelegate<TSagaData> action);

        ISagaBuilderThen<TSagaData> ThenAsync(ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation);

        ISagaBuilderThen<TSagaData> ThenAsync(string stepName, ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation);

        ISagaBuilderThen<TSagaData> ThenAsync<TSagaActivity>() where TSagaActivity : ISagaActivity<TSagaData>;

        ISagaBuilderThen<TSagaData> ThenAsync<TSagaActivity>(string stepName) where TSagaActivity : ISagaActivity<TSagaData>;

        ISagaBuilderWhen<TSagaData> TransitionTo<TState>() where TState : IState;
    }
}