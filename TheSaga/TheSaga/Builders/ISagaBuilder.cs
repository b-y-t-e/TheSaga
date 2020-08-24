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
        SagaBuilder<TSagaState> After(TimeSpan time);
        SagaModel<TSagaState> Build();
        SagaBuilder<TSagaState> During<TState>() where TState : IState;
        SagaBuilder<TSagaState> Start<TEvent>() where TEvent : IEvent;
        SagaBuilder<TSagaState> Then( ThenActionDelegate<TSagaState> action);
        SagaBuilder<TSagaState> Then(string stepName, ThenActionDelegate<TSagaState> action);
        SagaBuilder<TSagaState> Then<TSagaActivity>() where TSagaActivity : ISagaActivity<TSagaState>;
        SagaBuilder<TSagaState> Then<TSagaActivity>(string stepName) where TSagaActivity : ISagaActivity<TSagaState>;
        SagaBuilder<TSagaState> TransitionTo<TState>() where TState : IState;
        SagaBuilder<TSagaState> When<TEvent>() where TEvent : IEvent;
        SagaBuilder<TSagaState> When<TEvent, TEventHandler>() where TEvent : IEvent
           where TEventHandler : IEventHandler<TSagaState, TEvent>;
    }
}