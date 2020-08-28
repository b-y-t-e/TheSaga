using System;
using TheSaga.Activities;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.SagaModels;
using TheSaga.SagaModels.Steps.Delegates;
using TheSaga.States;

namespace TheSaga.Builders
{
    public interface ISagaBuilderThen<TSagaData> : ISagaBuilderWhen<TSagaData>
        where TSagaData : ISagaData
    {
        ISagaBuilderThen<TSagaData> After(TimeSpan time);

        ISagaModel<TSagaData> Build();

        ISagaBuilder<TSagaData> Finish();

        ISagaBuilderThen<TSagaData> Publish<TEvent>()
            where TEvent : IEvent, new();

        ISagaBuilderThen<TSagaData> Publish<TEvent>(SendActionDelegate<TSagaData, TEvent> action)
            where TEvent : IEvent, new();

        ISagaBuilderThen<TSagaData> Publish<TEvent, TCompensateEvent>()
            where TEvent : IEvent, new()
            where TCompensateEvent : IEvent, new();

        ISagaBuilderThen<TSagaData> Publish<TEvent, TCompensateEvent>(SendActionDelegate<TSagaData, TEvent> action, SendActionDelegate<TSagaData, TCompensateEvent> compensation)
            where TEvent : IEvent, new()
            where TCompensateEvent : IEvent, new();

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

        ISagaBuilderThen<TSagaData> TransitionTo<TState>() where TState : IState;
    }
}