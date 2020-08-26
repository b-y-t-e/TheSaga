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

        ISagaBuilderDuringState<TSagaData> During<TState>() where TState : IState;

        ISagaBuilderState<TSagaData> Start<TEvent>() where TEvent : IEvent;

        ISagaBuilderState<TSagaData> Start<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaData, TEvent>;

        ISagaBuilderState<TSagaData> Start<TEvent>(string stepName) where TEvent : IEvent;

        ISagaBuilderState<TSagaData> Start<TEvent, TEventHandler>(string stepName) where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaData, TEvent>;

        ISagaBuilderState<TSagaData> StartAsync<TEvent, TEventHandler>() where TEvent : IEvent
                           where TEventHandler : IEventHandler<TSagaData, TEvent>;

        ISagaBuilderState<TSagaData> StartAsync<TEvent, TEventHandler>(string stepName) where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaData, TEvent>;
    }

    public interface ISagaBuilderDuringState<TSagaData> : ISagaBuilderState<TSagaData>
        where TSagaData : ISagaData
    {
        ISagaBuilderState<TSagaData> When<TEvent>() where TEvent : IEvent;

        ISagaBuilderState<TSagaData> When<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaData, TEvent>;

        ISagaBuilderState<TSagaData> WhenAsync<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaData, TEvent>;
    }

    public interface ISagaBuilderState<TSagaData>
            where TSagaData : ISagaData
    {
        ISagaBuilderState<TSagaData> After(TimeSpan time);

        ISagaModel<TSagaData> Build();

        ISagaBuilder<TSagaData> Finish();

        ISagaBuilderState<TSagaData> Then(ThenActionDelegate<TSagaData> action);

        ISagaBuilderState<TSagaData> Then(string stepName, ThenActionDelegate<TSagaData> action);

        ISagaBuilderState<TSagaData> Then(ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation);

        ISagaBuilderState<TSagaData> Then(string stepName, ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation);

        ISagaBuilderState<TSagaData> Then<TSagaActivity>() where TSagaActivity : ISagaActivity<TSagaData>;

        ISagaBuilderState<TSagaData> Then<TSagaActivity>(string stepName) where TSagaActivity : ISagaActivity<TSagaData>;

        ISagaBuilderState<TSagaData> ThenAsync(ThenActionDelegate<TSagaData> action);

        ISagaBuilderState<TSagaData> ThenAsync(string stepName, ThenActionDelegate<TSagaData> action);

        ISagaBuilderState<TSagaData> ThenAsync(ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation);

        ISagaBuilderState<TSagaData> ThenAsync(string stepName, ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation);

        ISagaBuilderState<TSagaData> ThenAsync<TSagaActivity>() where TSagaActivity : ISagaActivity<TSagaData>;

        ISagaBuilderState<TSagaData> ThenAsync<TSagaActivity>(string stepName) where TSagaActivity : ISagaActivity<TSagaData>;

        ISagaBuilder<TSagaData> TransitionTo<TState>() where TState : IState;
    }
}