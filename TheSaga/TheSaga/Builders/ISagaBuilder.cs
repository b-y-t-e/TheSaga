using System;
using TheSaga.Activities;
using TheSaga.Interfaces;
using TheSaga.Models;
using TheSaga.States;
using TheSaga.States.Actions;

namespace TheSaga.Builders
{
    public interface ISagaBuilder<TSagaState> where TSagaState : ISagaState
    {
        ISagaBuilderDuringState<TSagaState> During<TState>() where TState : IState;
        ISagaBuilderState<TSagaState> Start<TEvent>() where TEvent : IEvent;
        ISagaBuilderState<TSagaState> Start<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaState, TEvent>;
        ISagaBuilderState<TSagaState> StartAsync<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaState, TEvent>;
        ISagaModel<TSagaState> Build();
    }

    public interface ISagaBuilderState<TSagaState>
        where TSagaState : ISagaState
    {
        ISagaBuilderState<TSagaState> After(TimeSpan time);
        ISagaBuilderState<TSagaState> Then(ThenActionDelegate<TSagaState> action);
        ISagaBuilderState<TSagaState> Then(string stepName, ThenActionDelegate<TSagaState> action);
        ISagaBuilderState<TSagaState> ThenAsync(ThenActionDelegate<TSagaState> action);
        ISagaBuilderState<TSagaState> ThenAsync(string stepName, ThenActionDelegate<TSagaState> action);
        ISagaBuilderState<TSagaState> Then<TSagaActivity>() where TSagaActivity : ISagaActivity<TSagaState>;
        ISagaBuilderState<TSagaState> Then<TSagaActivity>(string stepName) where TSagaActivity : ISagaActivity<TSagaState>;
        ISagaBuilderState<TSagaState> ThenAsync<TSagaActivity>() where TSagaActivity : ISagaActivity<TSagaState>;
        ISagaBuilderState<TSagaState> ThenAsync<TSagaActivity>(string stepName) where TSagaActivity : ISagaActivity<TSagaState>;
        ISagaModel<TSagaState> Build();
        ISagaBuilder<TSagaState> TransitionTo<TState>() where TState : IState;
        ISagaBuilder<TSagaState> Finish();
    }

    public interface ISagaBuilderDuringState<TSagaState> : ISagaBuilderState<TSagaState>
        where TSagaState : ISagaState
    {
        ISagaBuilderState<TSagaState> When<TEvent>() where TEvent : IEvent;
        ISagaBuilderState<TSagaState> When<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaState, TEvent>;
        ISagaBuilderState<TSagaState> WhenAsync<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaState, TEvent>;
    }
}