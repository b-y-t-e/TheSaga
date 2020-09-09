using System;
using TheSaga.Activities;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.ModelsHandlers;
using TheSaga.SagaModels;
using TheSaga.SagaModels.Steps.Delegates;
using TheSaga.States;

namespace TheSaga.Builders.Handlers
{
    public interface IHandlersBuilderThen : IHandlersBuilderWhen
    {
        IHandlersModel Build();

        IHandlersBuilder Finish();

        IHandlersBuilderThen Publish<TEvent>()
            where TEvent : IHandlersEvent, new();

        IHandlersBuilderThen Publish<TEvent>(HandlersSendActionDelegate<TEvent> action)
            where TEvent : IHandlersEvent, new();

        IHandlersBuilderThen Publish<TEvent, TCompensateEvent>()
            where TEvent : IHandlersEvent, new()
            where TCompensateEvent : IHandlersEvent, new();

        IHandlersBuilderThen Publish<TEvent, TCompensateEvent>(HandlersSendActionDelegate<TEvent> action,
            HandlersSendActionDelegate<TCompensateEvent> compensation)
            where TEvent : IHandlersEvent, new()
            where TCompensateEvent : IHandlersEvent, new();

        IHandlersBuilderThen Then(HandlersThenActionDelegate action);

        IHandlersBuilderThen Do(Action<IHandlersBuilderThen> builder);

        IHandlersBuilderThen If<TSagaCondition>(Action<IHandlersBuilderThen> builder);

        IHandlersBuilderThen Then(string stepName, HandlersThenActionDelegate action);

        IHandlersBuilderThen Then(HandlersThenActionDelegate action,
            HandlersThenActionDelegate compensation);

        IHandlersBuilderThen Then(string stepName, HandlersThenActionDelegate action,
            HandlersThenActionDelegate compensation);

        IHandlersBuilderThen Then<TSagaActivity>() where TSagaActivity : IHandlersActivity;

        IHandlersBuilderThen Then<TSagaActivity>(string stepName) where TSagaActivity : IHandlersActivity;

        IHandlersBuilderThen ThenAsync(HandlersThenActionDelegate action);

        IHandlersBuilderThen ThenAsync(string stepName, HandlersThenActionDelegate action);

        IHandlersBuilderThen ThenAsync(HandlersThenActionDelegate action,
            HandlersThenActionDelegate compensation);

        IHandlersBuilderThen ThenAsync(string stepName, HandlersThenActionDelegate action,
            HandlersThenActionDelegate compensation);

        IHandlersBuilderThen ThenAsync<TSagaActivity>() where TSagaActivity : IHandlersActivity;

        IHandlersBuilderThen ThenAsync<TSagaActivity>(string stepName)
            where TSagaActivity : IHandlersActivity;
    }
}