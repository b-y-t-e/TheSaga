using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Conditions;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Models;

using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga;
using TheSaga.ModelsSaga.Actions;
using TheSaga.ModelsSaga.Actions.Interfaces;
using TheSaga.Models.History;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.ModelsSaga.Steps;
using TheSaga.ModelsSaga.Steps.Delegates;
using TheSaga.States;
using TheSaga.States.Interfaces;
using TheSaga.Utils;

namespace TheSaga.Builders
{
    internal class SagaBuilder<TSagaData, TEvent> : SagaBuilder<TSagaData>,
        ISagaBuilder<TSagaData>,
        ISagaBuilderThen<TSagaData, TEvent>,
        ISagaBuilderWhen<TSagaData>
        where TSagaData : ISagaData
        where TEvent : ISagaEvent
    {
        public SagaBuilder(IServiceProvider serviceProvider) :
            base(serviceProvider)
        {
        }

        public SagaBuilder(SagaBuilderState sagaBuilderState) :
            base(sagaBuilderState)
        {
        }

        public ISagaBuilderThen<TSagaData, TEvent> Then(
            string stepName,
            ThenAsyncActionDelegate<TSagaData> action)
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    stepName,
                    action,
                    null,
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Then(
            ThenAsyncActionDelegate<TSagaData> action,
            ThenAsyncActionDelegate<TSagaData> compensation)
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Then)),
                    action,
                    compensation,
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Then(
            string stepName,
            ThenAsyncActionDelegate<TSagaData> action,
            ThenAsyncActionDelegate<TSagaData> compensation)
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    stepName,
                    action,
                    compensation,
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Then(
            ThenAsyncActionDelegate<TSagaData> action)
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Then)),
                    action,
                    null,
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Then(
            string stepName,
            ThenActionDelegate<TSagaData> action)
        {
            ThenActionDelegate<TSagaData> a = action;
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    stepName,
                    ctx => { a?.Invoke(ctx); return Task.CompletedTask; },
                    null,
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Then(
            ThenActionDelegate<TSagaData> action,
            ThenActionDelegate<TSagaData> compensation)
        {
            ThenActionDelegate<TSagaData> a = action;
            ThenActionDelegate<TSagaData> c = compensation;

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Then)),
                    ctx => { a?.Invoke(ctx); return Task.CompletedTask; },
                    ctx => { c?.Invoke(ctx); return Task.CompletedTask; },
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Then(
            string stepName,
            ThenActionDelegate<TSagaData> action,
            ThenActionDelegate<TSagaData> compensation)
        {
            ThenActionDelegate<TSagaData> a = action;
            ThenActionDelegate<TSagaData> c = compensation;
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    stepName,
                    ctx => { a?.Invoke(ctx); return Task.CompletedTask; },
                    ctx => { c?.Invoke(ctx); return Task.CompletedTask; },
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }


        public ISagaBuilderThen<TSagaData, TEvent> Then(
            ThenActionDelegate<TSagaData> action)
        {
            ThenActionDelegate<TSagaData> a = action;

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Then)),
                    ctx => { a?.Invoke(ctx); return Task.CompletedTask; },
                    null,
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }


        public ISagaBuilderThen<TSagaData, TEvent> ThenAsync<TSagaActivity>()
            where TSagaActivity : ISagaActivity<TSagaData>
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThen<TSagaData, TSagaActivity>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(ThenAsync), typeof(TSagaActivity).Name),
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> ThenAsync<TSagaActivity>(string stepName)
            where TSagaActivity : ISagaActivity<TSagaData>
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThen<TSagaData, TSagaActivity>(
                    stepName,
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> ThenAsync(
            ThenAsyncActionDelegate<TSagaData> action)
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(ThenAsync)),
                    action,
                    null,
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> ThenAsync(
            string stepName,
            ThenAsyncActionDelegate<TSagaData> action)
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    stepName,
                    action,
                    null,
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> ThenAsync(
            ThenAsyncActionDelegate<TSagaData> action,
            ThenAsyncActionDelegate<TSagaData> compensation)
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(ThenAsync)),
                    action,
                    compensation,
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> ThenAsync(
            string stepName,
            ThenAsyncActionDelegate<TSagaData> action,
            ThenAsyncActionDelegate<TSagaData> compensation)
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    stepName,
                    action,
                    compensation,
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }


        public ISagaBuilderThen<TSagaData, TEvent> ThenAsync(
            ThenActionDelegate<TSagaData> action)
        {
            ThenActionDelegate<TSagaData> a = action;

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(ThenAsync)),
                    ctx => { a?.Invoke(ctx); return Task.CompletedTask; },
                    null,
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> ThenAsync(
            string stepName,
            ThenActionDelegate<TSagaData> action)
        {
            ThenActionDelegate<TSagaData> a = action;
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    stepName,
                    ctx => { a?.Invoke(ctx); return Task.CompletedTask; },
                    null,
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> ThenAsync(
            ThenActionDelegate<TSagaData> action,
            ThenActionDelegate<TSagaData> compensation)
        {
            ThenActionDelegate<TSagaData> a = action;
            ThenActionDelegate<TSagaData> c = compensation;

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(ThenAsync)),
                    ctx => { a?.Invoke(ctx); return Task.CompletedTask; },
                    ctx => { c?.Invoke(ctx); return Task.CompletedTask; },
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> ThenAsync(
            string stepName,
            ThenActionDelegate<TSagaData> action,
            ThenActionDelegate<TSagaData> compensation)
        {
            ThenActionDelegate<TSagaData> a = action;
            ThenActionDelegate<TSagaData> c = compensation;
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    stepName,
                    ctx => { a?.Invoke(ctx); return Task.CompletedTask; },
                    ctx => { c?.Invoke(ctx); return Task.CompletedTask; },
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }


        public ISagaBuilderThen<TSagaData, TEvent> TransitionTo<TState>() where TState : ISagaState
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(TransitionTo), typeof(TState).Name),
                    ctx =>
                    {
                        SagaExecutionState executionState = ctx.ExecutionState as SagaExecutionState;
                        executionState.CurrentState = typeof(TState).Name;
                        return Task.CompletedTask;
                    },
                    ctx =>
                    {
                        SagaExecutionState executionState = ctx.ExecutionState as SagaExecutionState;
                        StepData data = executionState.CurrentStepData();
                        executionState.CurrentState = data.StateName;
                        return Task.CompletedTask;
                    },
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Publish<TEventToSend, TCompensateEvent>()
            where TEventToSend : ISagaEvent, new()
            where TCompensateEvent : ISagaEvent, new()
        {
            ISagaPublishActivity<TSagaData, TEventToSend, TCompensateEvent> publishActivity = (ISagaPublishActivity<TSagaData, TEventToSend, TCompensateEvent>)builderState.ServiceProvider.
                GetService(typeof(ISagaPublishActivity<TSagaData, TEventToSend, TCompensateEvent>));

            if (publishActivity == null)            
                publishActivity = new SagaStepForPublishActivity<TSagaData, TEventToSend, TCompensateEvent>();

            publishActivity.StepName = builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Publish), typeof(TEventToSend).Name);
            publishActivity.Async = false;
            publishActivity.ParentStep = builderState.ParentStep;

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                publishActivity);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Publish<TEventToSend, TCompensateEvent>(
            SendActionAsyncDelegate<TSagaData, TEventToSend> action,
            SendActionAsyncDelegate<TSagaData, TCompensateEvent> compensation)
            where TEventToSend : ISagaEvent, new()
            where TCompensateEvent : ISagaEvent, new()
        {
            ISagaPublishActivity<TSagaData, TEventToSend, TCompensateEvent> publishActivity = (ISagaPublishActivity<TSagaData, TEventToSend, TCompensateEvent>)builderState.ServiceProvider.
                GetService(typeof(ISagaPublishActivity<TSagaData, TEventToSend, TCompensateEvent>));

            if (publishActivity == null)            
                publishActivity = new SagaStepForPublishActivity<TSagaData, TEventToSend, TCompensateEvent>();

            publishActivity.ActionDelegate = action;
            publishActivity.CompensateDelegate = compensation;
            publishActivity.StepName = builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Publish), typeof(TEventToSend).Name);
            publishActivity.Async = false;
            publishActivity.ParentStep = builderState.ParentStep;

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                publishActivity);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Publish<TEventToSend, TCompensateEvent>(
            SendActionDelegate<TSagaData, TEventToSend> action,
            SendActionDelegate<TSagaData, TCompensateEvent> compensation)
            where TEventToSend : ISagaEvent, new()
            where TCompensateEvent : ISagaEvent, new()
        {
            SendActionDelegate<TSagaData, TEventToSend> a = action;
            SendActionDelegate<TSagaData, TCompensateEvent> c = compensation;
            
            ISagaPublishActivity<TSagaData, TEventToSend, TCompensateEvent> publishActivity = (ISagaPublishActivity<TSagaData, TEventToSend, TCompensateEvent>)builderState.ServiceProvider.
                GetService(typeof(ISagaPublishActivity<TSagaData, TEventToSend, TCompensateEvent>));

            if (publishActivity == null)
                publishActivity = new SagaStepForPublishActivity<TSagaData, TEventToSend, TCompensateEvent>();

            publishActivity.ActionDelegate = (ctx, ev) => { a?.Invoke(ctx, ev); return Task.CompletedTask; };
            publishActivity.CompensateDelegate = (ctx, ev) => { c?.Invoke(ctx, ev); return Task.CompletedTask; };
            publishActivity.StepName = builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Publish), typeof(TEventToSend).Name);
            publishActivity.Async = false;
            publishActivity.ParentStep = builderState.ParentStep;

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                publishActivity);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Publish<TEventToSend>()
            where TEventToSend : ISagaEvent, new()
        {
            ISagaPublishActivity<TSagaData, TEventToSend, EmptyEvent> publishActivity = (ISagaPublishActivity<TSagaData, TEventToSend, EmptyEvent>)builderState.ServiceProvider.
                GetService(typeof(ISagaPublishActivity<TSagaData, TEventToSend, EmptyEvent>));

            if (publishActivity == null)
                publishActivity = new SagaStepForPublishActivity<TSagaData, TEventToSend, EmptyEvent>();

            publishActivity.ActionDelegate = null;
            publishActivity.CompensateDelegate = null;
            publishActivity.StepName = builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Publish), typeof(TEventToSend).Name);
            publishActivity.Async = false;
            publishActivity.ParentStep = builderState.ParentStep;

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                publishActivity);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Abort()
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForBreak<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Abort)),
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Publish<TEventToSend>(
            SendActionAsyncDelegate<TSagaData, TEventToSend> action)
            where TEventToSend : ISagaEvent, new()
        {
            ISagaPublishActivity<TSagaData, TEventToSend, EmptyEvent> publishActivity = (ISagaPublishActivity<TSagaData, TEventToSend, EmptyEvent>)builderState.ServiceProvider.
                GetService(typeof(ISagaPublishActivity<TSagaData, TEventToSend, EmptyEvent>));

            if (publishActivity == null)
                publishActivity = new SagaStepForPublishActivity<TSagaData, TEventToSend, EmptyEvent>();

            publishActivity.ActionDelegate = action;
            publishActivity.CompensateDelegate = null;
            publishActivity.StepName = builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Publish), typeof(TEventToSend).Name);
            publishActivity.Async = false;
            publishActivity.ParentStep = builderState.ParentStep;

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                publishActivity);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Publish<TEventToSend>(
            SendActionDelegate<TSagaData, TEventToSend> action)
            where TEventToSend : ISagaEvent, new()
        {
            SendActionDelegate<TSagaData, TEventToSend> a = action;

            ISagaPublishActivity<TSagaData, TEventToSend, EmptyEvent> publishActivity = (ISagaPublishActivity<TSagaData, TEventToSend, EmptyEvent>)builderState.ServiceProvider.
                GetService(typeof(ISagaPublishActivity<TSagaData, TEventToSend, EmptyEvent>));

            if (publishActivity == null)
                publishActivity = new SagaStepForPublishActivity<TSagaData, TEventToSend, EmptyEvent>();

            publishActivity.ActionDelegate = (ctx, ev) => { a?.Invoke(ctx, ev); return Task.CompletedTask; };
            publishActivity.CompensateDelegate = null;
            publishActivity.StepName = builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Publish), typeof(TEventToSend).Name);
            publishActivity.Async = false;
            publishActivity.ParentStep = builderState.ParentStep;

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                publishActivity);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> HandleBy<TEventHandler>(string stepName)
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            if (builderState.CurrentEvent == null)
                throw new Exception($"{nameof(HandleBy)} must be defined after {nameof(When)} / {nameof(WhenAsync)}");

            ISagaAction action = builderState.Model.
                FindActionByStateAndEventType(builderState.CurrentState, builderState.CurrentEvent);

            action.ChildSteps.RemoveEmptyStepsAtBeginning();
            action.ChildSteps.AddStep(new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                stepName,
                false,
                builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> HandleByAsync<TEventHandler>(string stepName)
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            if (builderState.CurrentEvent == null)
                throw new Exception($"{nameof(HandleBy)} must be defined after {nameof(When)} / {nameof(WhenAsync)}");

            ISagaAction action = builderState.Model.
                FindActionByStateAndEventType(builderState.CurrentState, builderState.CurrentEvent);

            action.ChildSteps.RemoveEmptyStepsAtBeginning();
            action.ChildSteps.AddStep(new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                stepName,
                true,
                builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> HandleBy<TEventHandler>()
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>
        {
            if (builderState.CurrentEvent == null)
                throw new Exception($"{nameof(HandleBy)} must be defined after {nameof(When)} / {nameof(WhenAsync)}");

            ISagaAction action = builderState.Model.
                FindActionByStateAndEventType(builderState.CurrentState, builderState.CurrentEvent);

            action.ChildSteps.RemoveEmptyStepsAtBeginning();
            action.ChildSteps.AddStep(new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(HandleBy), builderState.CurrentEvent.GetType().Name,
                    typeof(TEventHandler).Name),
                false,
                builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> HandleByAsync<TEventHandler>()
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>
        {
            if (builderState.CurrentEvent == null)
                throw new Exception($"{nameof(HandleBy)} must be defined after {nameof(When)} / {nameof(WhenAsync)}");

            ISagaAction action = builderState.Model.
                FindActionByStateAndEventType(builderState.CurrentState, builderState.CurrentEvent);

            action.ChildSteps.RemoveEmptyStepsAtBeginning();
            action.ChildSteps.AddStep(new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(HandleByAsync), builderState.CurrentEvent.GetType().Name,
                typeof(TEventHandler).Name),
                true,
                builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> If<TSagaCondition>(
            Action<ISagaBuilderThen<TSagaData, TEvent>> builderAction)
            where TSagaCondition : ISagaCondition<TSagaData>
        {
            SagaAction currentAction = builderState.CurrentAction;

            SagaStepForIf<TSagaData, TSagaCondition> parentStep = new SagaStepForIf<TSagaData, TSagaCondition>(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(If)),
                builderState.ParentStep);

            SagaBuilderState childBuildState = new SagaBuilderState(
                null, null, new SagaModel(typeof(TSagaData)),
                builderState.ServiceProvider,
                builderState.UniqueNameGenerator,
                parentStep);

            SagaBuilder<TSagaData, TEvent> childBuilder = new SagaBuilder<TSagaData, TEvent>(childBuildState);
            childBuilder.Start<EmptyEvent>();

            builderAction(childBuilder);

            ISagaAction mainChildAction = childBuilder.
                builderState.Model.Actions.
                FindActionByStateAndEventType(new SagaStartState().GetStateName(), typeof(EmptyEvent));

            mainChildAction.
                ChildSteps.RemoveEmptyStepsAtBeginning();

            parentStep.
                SetChildSteps(mainChildAction.ChildSteps);

            currentAction.
                ChildSteps.AddStep(parentStep);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }
        public ISagaBuilderThen<TSagaData, TEvent> If(
            IfFuncDelegate<TSagaData> condition,
            Action<ISagaBuilderThen<TSagaData, TEvent>> builderAction)
        {
            IfFuncDelegate<TSagaData> c = condition;
            SagaAction currentAction = builderState.CurrentAction;

            SagaStepForIfInline<TSagaData> parentStep = new SagaStepForIfInline<TSagaData>(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(If)),
                ctx => Task.FromResult(c?.Invoke(ctx) ?? false),
                null,
                builderState.ParentStep);

            SagaBuilderState childBuildState = new SagaBuilderState(
                null, null, new SagaModel(typeof(TSagaData)),
                builderState.ServiceProvider,
                builderState.UniqueNameGenerator,
                parentStep);

            SagaBuilder<TSagaData, TEvent> childBuilder = new SagaBuilder<TSagaData, TEvent>(childBuildState);
            childBuilder.Start<EmptyEvent>();

            builderAction(childBuilder);

            ISagaAction mainChildAction = childBuilder.
                builderState.Model.Actions.
                FindActionByStateAndEventType(new SagaStartState().GetStateName(), typeof(EmptyEvent));

            mainChildAction.
                ChildSteps.RemoveEmptyStepsAtBeginning();

            parentStep.
                SetChildSteps(mainChildAction.ChildSteps);

            currentAction.
                ChildSteps.AddStep(parentStep);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }
        public ISagaBuilderThen<TSagaData, TEvent> If(
            IfFuncAsyncDelegate<TSagaData> condition,
            Action<ISagaBuilderThen<TSagaData, TEvent>> builderAction)
        {
            SagaAction currentAction = builderState.CurrentAction;

            SagaStepForIfInline<TSagaData> parentStep = new SagaStepForIfInline<TSagaData>(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(If)),
                condition,
                null,
                builderState.ParentStep);

            SagaBuilderState childBuildState = new SagaBuilderState(
                null, null, new SagaModel(typeof(TSagaData)),
                builderState.ServiceProvider,
                builderState.UniqueNameGenerator,
                parentStep);

            SagaBuilder<TSagaData, TEvent> childBuilder = new SagaBuilder<TSagaData, TEvent>(childBuildState);
            childBuilder.Start<EmptyEvent>();

            builderAction(childBuilder);

            ISagaAction mainChildAction = childBuilder.
                builderState.Model.Actions.
                FindActionByStateAndEventType(new SagaStartState().GetStateName(), typeof(EmptyEvent));

            mainChildAction.
                ChildSteps.RemoveEmptyStepsAtBeginning();

            parentStep.
                SetChildSteps(mainChildAction.ChildSteps);

            currentAction.
                ChildSteps.AddStep(parentStep);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> While<TSagaCondition>(
            Action<ISagaBuilderThen<TSagaData, TEvent>> builderAction)
            where TSagaCondition : ISagaCondition<TSagaData>
        {
            SagaAction currentAction = builderState.CurrentAction;

            SagaStepForWhile<TSagaData, TSagaCondition> parentStep = new SagaStepForWhile<TSagaData, TSagaCondition>(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(While)),
                builderState.ParentStep);

            SagaBuilderState childBuildState = new SagaBuilderState(
                null, null, new SagaModel(typeof(TSagaData)),
                builderState.ServiceProvider,
                builderState.UniqueNameGenerator,
                parentStep);

            SagaBuilder<TSagaData, TEvent> childBuilder = new SagaBuilder<TSagaData, TEvent>(childBuildState);
            childBuilder.Start<EmptyEvent>();

            builderAction(childBuilder);

            ISagaAction mainChildAction = childBuilder.
                builderState.Model.Actions.
                FindActionByStateAndEventType(new SagaStartState().GetStateName(), typeof(EmptyEvent));

            mainChildAction.
                ChildSteps.RemoveEmptyStepsAtBeginning();

            parentStep.
                SetChildSteps(mainChildAction.ChildSteps);

            currentAction.
                ChildSteps.AddStep(parentStep);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }
        public ISagaBuilderThen<TSagaData, TEvent> While(
            IfFuncAsyncDelegate<TSagaData> condition,
            Action<ISagaBuilderThen<TSagaData, TEvent>> builderAction)
        {
            SagaAction currentAction = builderState.CurrentAction;

            SagaStepForWhileInline<TSagaData> parentStep = new SagaStepForWhileInline<TSagaData>(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(While)),
                condition,
                null,
                builderState.ParentStep);

            SagaBuilderState childBuildState = new SagaBuilderState(
                null, null, new SagaModel(typeof(TSagaData)),
                builderState.ServiceProvider,
                builderState.UniqueNameGenerator,
                parentStep);

            SagaBuilder<TSagaData, TEvent> childBuilder = new SagaBuilder<TSagaData, TEvent>(childBuildState);
            childBuilder.Start<EmptyEvent>();

            builderAction(childBuilder);

            ISagaAction mainChildAction = childBuilder.
                builderState.Model.Actions.
                FindActionByStateAndEventType(new SagaStartState().GetStateName(), typeof(EmptyEvent));

            mainChildAction.
                ChildSteps.RemoveEmptyStepsAtBeginning();

            parentStep.
                SetChildSteps(mainChildAction.ChildSteps);

            currentAction.
                ChildSteps.AddStep(parentStep);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }
        public ISagaBuilderThen<TSagaData, TEvent> While(
            IfFuncDelegate<TSagaData> condition,
            Action<ISagaBuilderThen<TSagaData, TEvent>> builderAction)
        {
            IfFuncDelegate<TSagaData> c = condition;
            SagaAction currentAction = builderState.CurrentAction;

            SagaStepForWhileInline<TSagaData> parentStep = new SagaStepForWhileInline<TSagaData>(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(While)),
                ctx => Task.FromResult(c?.Invoke(ctx) ?? false),
                null,
                builderState.ParentStep);

            SagaBuilderState childBuildState = new SagaBuilderState(
                null, null, new SagaModel(typeof(TSagaData)),
                builderState.ServiceProvider,
                builderState.UniqueNameGenerator,
                parentStep);

            SagaBuilder<TSagaData, TEvent> childBuilder = new SagaBuilder<TSagaData, TEvent>(childBuildState);
            childBuilder.Start<EmptyEvent>();

            builderAction(childBuilder);

            ISagaAction mainChildAction = childBuilder.
                builderState.Model.Actions.
                FindActionByStateAndEventType(new SagaStartState().GetStateName(), typeof(EmptyEvent));

            mainChildAction.
                ChildSteps.RemoveEmptyStepsAtBeginning();

            parentStep.
                SetChildSteps(mainChildAction.ChildSteps);

            currentAction.
                ChildSteps.AddStep(parentStep);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }
        public ISagaBuilderThen<TSagaData, TEvent> ElseIf(
            IfFuncDelegate<TSagaData> condition,
            Action<ISagaBuilderThen<TSagaData, TEvent>> builderAction)
        {
            IfFuncDelegate<TSagaData> c = condition;
            SagaAction currentAction = builderState.CurrentAction;

            SagaStepForElseIfInline<TSagaData> parentStep = new SagaStepForElseIfInline<TSagaData>(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(ElseIf)),
                ctx => Task.FromResult(c?.Invoke(ctx) ?? false),
                null,
                builderState.ParentStep);

            SagaBuilderState childBuildState = new SagaBuilderState(
                null, null, new SagaModel(typeof(TSagaData)),
                builderState.ServiceProvider,
                builderState.UniqueNameGenerator,
                parentStep);

            SagaBuilder<TSagaData, TEvent> childBuilder = new SagaBuilder<TSagaData, TEvent>(childBuildState);
            childBuilder.Start<EmptyEvent>();

            builderAction(childBuilder);

            ISagaAction mainChildAction = childBuilder.
                builderState.Model.Actions.
                FindActionByStateAndEventType(new SagaStartState().GetStateName(), typeof(EmptyEvent));

            mainChildAction.
                ChildSteps.RemoveEmptyStepsAtBeginning();

            parentStep.
                SetChildSteps(mainChildAction.ChildSteps);

            currentAction.
                ChildSteps.AddStep(parentStep);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }
        public ISagaBuilderThen<TSagaData, TEvent> ElseIf(
            IfFuncAsyncDelegate<TSagaData> condition,
            Action<ISagaBuilderThen<TSagaData, TEvent>> builderAction)
        {
            SagaAction currentAction = builderState.CurrentAction;

            SagaStepForElseIfInline<TSagaData> parentStep = new SagaStepForElseIfInline<TSagaData>(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(ElseIf)),
                condition,
                null,
                builderState.ParentStep);

            SagaBuilderState childBuildState = new SagaBuilderState(
                null, null, new SagaModel(typeof(TSagaData)),
                builderState.ServiceProvider,
                builderState.UniqueNameGenerator,
                parentStep);

            SagaBuilder<TSagaData, TEvent> childBuilder = new SagaBuilder<TSagaData, TEvent>(childBuildState);
            childBuilder.Start<EmptyEvent>();

            builderAction(childBuilder);

            ISagaAction mainChildAction = childBuilder.
                builderState.Model.Actions.
                FindActionByStateAndEventType(new SagaStartState().GetStateName(), typeof(EmptyEvent));

            mainChildAction.
                ChildSteps.RemoveEmptyStepsAtBeginning();

            parentStep.
                SetChildSteps(mainChildAction.ChildSteps);

            currentAction.
                ChildSteps.AddStep(parentStep);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }
        public ISagaBuilderThen<TSagaData, TEvent> Else(Action<ISagaBuilderThen<TSagaData, TEvent>> builderAction)
        {
            SagaAction currentAction = builderState.CurrentAction;

            SagaStepForElse<TSagaData> parentStep = new SagaStepForElse<TSagaData>(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Else)),
                builderState.ParentStep);

            SagaBuilderState childBuildState = new SagaBuilderState(
                null, null, new SagaModel(typeof(TSagaData)),
                builderState.ServiceProvider,
                builderState.UniqueNameGenerator,
                parentStep);

            SagaBuilder<TSagaData, TEvent> childBuilder = new SagaBuilder<TSagaData, TEvent>(childBuildState);
            childBuilder.Start<EmptyEvent>();

            builderAction(childBuilder);

            ISagaAction mainChildAction = childBuilder.
                builderState.Model.Actions.
                FindActionByStateAndEventType(new SagaStartState().GetStateName(), typeof(EmptyEvent));

            mainChildAction.
                ChildSteps.RemoveEmptyStepsAtBeginning();

            parentStep.
                SetChildSteps(mainChildAction.ChildSteps);

            currentAction.
                ChildSteps.AddStep(parentStep);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Do(Action<ISagaBuilderThen<TSagaData, TEvent>> builderAction)
        {
            SagaAction currentAction = builderState.CurrentAction;

            SagaContainerStep parentStep = new SagaContainerStep(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Do)),
                builderState.ParentStep);

            SagaBuilderState childBuildState = new SagaBuilderState(
                null, null, new SagaModel(typeof(TSagaData)),
                builderState.ServiceProvider,
                builderState.UniqueNameGenerator,
                parentStep);

            SagaBuilder<TSagaData, TEvent> childBuilder = new SagaBuilder<TSagaData, TEvent>(childBuildState);
            childBuilder.Start<EmptyEvent>();

            builderAction(childBuilder);

            ISagaAction mainChildAction = childBuilder.
                builderState.Model.Actions.
                FindActionByStateAndEventType(new SagaStartState().GetStateName(), typeof(EmptyEvent));

            mainChildAction.
                ChildSteps.RemoveEmptyStepsAtBeginning();

            parentStep.
                SetChildSteps(mainChildAction.ChildSteps);

            currentAction.
                ChildSteps.AddStep(parentStep);

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> After(TimeSpan time)
        {
            throw new NotImplementedException();
        }
        public ISagaBuilderThen<TSagaData, TEvent> Then<TSagaActivity>()
            where TSagaActivity : ISagaActivity<TSagaData>
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThen<TSagaData, TSagaActivity>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Then), typeof(TSagaActivity).Name),
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Then<TSagaActivity>(string stepName)
            where TSagaActivity : ISagaActivity<TSagaData>
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThen<TSagaData, TSagaActivity>(
                    stepName,
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

    }

    internal class SagaBuilder<TSagaData> :
        ISagaBuilder<TSagaData>,
        ISagaBuilderWhen<TSagaData>
        where TSagaData : ISagaData
    {
        protected SagaBuilderState builderState;

        public SagaBuilder(IServiceProvider serviceProvider)
        {
            builderState = new SagaBuilderState(
                null,
                null,
                new SagaModel(typeof(TSagaData)),
                serviceProvider,
                new UniqueNameGenerator(),
                null);
        }

        public SagaBuilder(SagaBuilderState sagaBuilderState)
        {
            builderState = sagaBuilderState;
        }

        public ISagaModel Build()
        {
            if (string.IsNullOrEmpty(builderState.Model.Name))
                throw new InvalidSagaModelNameException();

            return builderState.Model;
        }

        public ISagaBuilderWhen<TSagaData> During<TState>()
            where TState : ISagaState
        {
            builderState.CurrentState = typeof(TState).Name;
            builderState.CurrentEvent = null;
            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilder<TSagaData> Name(string name)
        {
            builderState.Model.Name = name;
            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Start<TEvent>()
            where TEvent : ISagaEvent
        {
            builderState.CurrentState = new SagaStartState().GetStateName();
            builderState.CurrentEvent = typeof(TEvent);
            SagaAction action = new SagaAction
            {
                State = builderState.CurrentState,
                Event = typeof(TEvent),
                ChildSteps = new SagaSteps(
                    new SagaEmptyStep(
                        builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Start), typeof(TEvent).Name),
                        builderState.ParentStep)
                )
            };
            builderState.CurrentAction = action;
            builderState.Model.Actions.Add(action);
            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Start<TEvent, TEventHandler>()
            where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>
        {
            builderState.CurrentState = new SagaStartState().GetStateName();
            builderState.CurrentEvent = typeof(TEvent);
            var action = new SagaAction
            {
                State = builderState.CurrentState,
                Event = typeof(TEvent),
                ChildSteps = new SagaSteps(
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Start), typeof(TEvent).Name,
                            typeof(TEventHandler).Name),
                        false,
                        builderState.ParentStep)
                )
            };
            builderState.CurrentAction = action;
            builderState.Model.Actions.Add(action);
            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Start<TEvent>(string stepName)
            where TEvent : ISagaEvent
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.CurrentState = new SagaStartState().GetStateName();
            builderState.CurrentEvent = typeof(TEvent);
            var action = new SagaAction
            {
                State = builderState.CurrentState,
                Event = typeof(TEvent),
                ChildSteps = new SagaSteps(
                    new SagaEmptyStep(
                        stepName,
                        builderState.ParentStep)
                )
            };
            builderState.CurrentAction = action;
            builderState.Model.Actions.Add(action);
            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> Start<TEvent, TEventHandler>(string stepName)
            where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.CurrentState = new SagaStartState().GetStateName();
            builderState.CurrentEvent = typeof(TEvent);
            var action = new SagaAction
            {
                State = builderState.CurrentState,
                Event = typeof(TEvent),
                ChildSteps = new SagaSteps(
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        stepName,
                        false,
                        builderState.ParentStep)
                )
            };
            builderState.CurrentAction = action;
            builderState.Model.Actions.Add(action);
            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> StartAsync<TEvent, TEventHandler>()
            where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>
        {
            builderState.CurrentState = new SagaStartState().GetStateName();
            builderState.CurrentEvent = typeof(TEvent);
            var action = new SagaAction
            {
                State = builderState.CurrentState,
                Event = typeof(TEvent),
                ChildSteps = new SagaSteps(
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(StartAsync), typeof(TEvent).Name,
                            typeof(TEventHandler).Name),
                        true,
                        builderState.ParentStep)
                )
            };
            builderState.CurrentAction = action;
            builderState.Model.Actions.Add(action);
            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> StartAsync<TEvent, TEventHandler>(string stepName)
            where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.CurrentState = new SagaStartState().GetStateName();
            builderState.CurrentEvent = typeof(TEvent);
            var action = new SagaAction
            {
                State = builderState.CurrentState,
                Event = typeof(TEvent),
                ChildSteps = new SagaSteps(
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        stepName,
                        true,
                    builderState.ParentStep)
                )
            };
            builderState.CurrentAction = action;
            builderState.Model.Actions.Add(action);
            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }


        public ISagaBuilder<TSagaData> Finish()
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Finish)),
                    ctx =>
                    {
                        SagaExecutionState executionState = ctx.ExecutionState as SagaExecutionState;
                        executionState.CurrentState = new SagaFinishState().GetStateName();
                        executionState.CurrentStep = null;
                        executionState.IsCompensating = false;
                        return Task.CompletedTask;
                    },
                    null,
                    false,
                    builderState.ParentStep));
            return new SagaBuilder<TSagaData>(builderState);
        }


        public ISagaBuilderThen<TSagaData, TEvent> When<TEvent>() where TEvent : ISagaEvent
        {
            builderState.CurrentEvent = typeof(TEvent);
            var action = new SagaAction
            {
                State = builderState.CurrentState,
                Event = builderState.CurrentEvent,
                ChildSteps = new SagaSteps(
                    new SagaEmptyStep(
                        builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(When), typeof(TEvent).Name),
                        builderState.ParentStep)
                )
            };
            builderState.CurrentAction = action;
            builderState.Model.Actions.Add(action);
            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> When<TEvent, TEventHandler>() where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>
        {
            builderState.CurrentEvent = typeof(TEvent);
            var action = new SagaAction
            {
                State = builderState.CurrentState,
                Event = builderState.CurrentEvent,
                ChildSteps = new SagaSteps(
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(When), typeof(TEvent).Name,
                            typeof(TEventHandler).Name),
                        false,
                        builderState.ParentStep)
                )
            };
            builderState.CurrentAction = action;
            builderState.Model.Actions.Add(action);
            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }

        public ISagaBuilderThen<TSagaData, TEvent> WhenAsync<TEvent, TEventHandler>() where TEvent : ISagaEvent
            where TEventHandler : ISagaEventHandler<TSagaData, TEvent>
        {
            builderState.CurrentEvent = typeof(TEvent);
            var action = new SagaAction
            {
                State = builderState.CurrentState,
                Event = builderState.CurrentEvent,
                ChildSteps = new SagaSteps(
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(WhenAsync), typeof(TEvent).Name,
                            typeof(TEventHandler).Name),
                        true,
                        builderState.ParentStep)
                )
            };
            builderState.CurrentAction = action;
            builderState.Model.Actions.Add(action);
            return new SagaBuilder<TSagaData, TEvent>(builderState);
        }
        public ISagaBuilder<TSagaData> Settings(Action<ISagaSettingsBuilder> settingsBuilder)
        {
            ISagaSettingsBuilder sagaSettingsBuilder = new SagaSettingsBuilder(builderState.Model);
            settingsBuilder(sagaSettingsBuilder);
            return new SagaBuilder<TSagaData>(builderState);
        }
    }
}
