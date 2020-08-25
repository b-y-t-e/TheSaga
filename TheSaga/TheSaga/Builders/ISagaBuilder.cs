using System;
using TheSaga.Activities;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Steps;
using TheSaga.States;

namespace TheSaga.Builders
{
    public interface ISagaBuilder<TSagaState> where TSagaState : ISagaState
    {
        ISagaModel<TSagaState> Build();

        ISagaBuilderDuringState<TSagaState> During<TState>() where TState : IState;

        ISagaBuilderState<TSagaState> Start<TEvent>() where TEvent : IEvent;

        ISagaBuilderState<TSagaState> Start<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaState, TEvent>;

        ISagaBuilderState<TSagaState> StartAsync<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaState, TEvent>;
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

    public interface ISagaBuilderState<TSagaState>
            where TSagaState : ISagaState
    {
        ISagaBuilderState<TSagaState> After(TimeSpan time);

        ISagaModel<TSagaState> Build();

        ISagaBuilder<TSagaState> Finish();

        ISagaBuilderState<TSagaState> Then(ThenActionDelegate<TSagaState> action);

        ISagaBuilderState<TSagaState> Then(string stepName, ThenActionDelegate<TSagaState> action);

        ISagaBuilderState<TSagaState> Then<TSagaActivity>() where TSagaActivity : ISagaActivity<TSagaState>;

        ISagaBuilderState<TSagaState> Then<TSagaActivity>(string stepName) where TSagaActivity : ISagaActivity<TSagaState>;

        ISagaBuilderState<TSagaState> ThenAsync(ThenActionDelegate<TSagaState> action);

        ISagaBuilderState<TSagaState> ThenAsync(string stepName, ThenActionDelegate<TSagaState> action);

        ISagaBuilderState<TSagaState> ThenAsync<TSagaActivity>() where TSagaActivity : ISagaActivity<TSagaState>;

        ISagaBuilderState<TSagaState> ThenAsync<TSagaActivity>(string stepName) where TSagaActivity : ISagaActivity<TSagaState>;

        ISagaBuilder<TSagaState> TransitionTo<TState>() where TState : IState;
    }
}