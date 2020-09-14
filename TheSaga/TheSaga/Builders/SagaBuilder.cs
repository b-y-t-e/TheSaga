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
using TheSaga.ModelsSaga.History;
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
        ISagaBuilderThen<TSagaData>,
        ISagaBuilderWhen<TSagaData>,
        ISagaBuilderHandle<TSagaData, TEvent>
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

        public ISagaBuilderHandle<TSagaData, TEvent> HandleBy<TEventHandler>(string stepName)
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

        public ISagaBuilderHandle<TSagaData, TEvent> HandleByAsync<TEventHandler>(string stepName)
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

        public ISagaBuilderHandle<TSagaData, TEvent> HandleBy<TEventHandler>()
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

        public ISagaBuilderHandle<TSagaData, TEvent> HandleByAsync<TEventHandler>()
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
    }

    internal class SagaBuilder<TSagaData> :
        ISagaBuilder<TSagaData>,
        ISagaBuilderThen<TSagaData>,
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

        public ISagaBuilderHandle<TSagaData, TEvent> Start<TEvent>()
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

        public ISagaBuilderThen<TSagaData> Start<TEvent, TEventHandler>()
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
            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderHandle<TSagaData, TEvent> Start<TEvent>(string stepName)
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

        public ISagaBuilderThen<TSagaData> Start<TEvent, TEventHandler>(string stepName)
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
            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> StartAsync<TEvent, TEventHandler>()
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
            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> StartAsync<TEvent, TEventHandler>(string stepName)
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
            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> After(TimeSpan time)
        {
            throw new NotImplementedException();
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

        public ISagaBuilderThen<TSagaData> Then<TSagaActivity>()
            where TSagaActivity : ISagaActivity<TSagaData>
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThen<TSagaData, TSagaActivity>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Then), typeof(TSagaActivity).Name),
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> Then<TSagaActivity>(string stepName)
            where TSagaActivity : ISagaActivity<TSagaData>
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThen<TSagaData, TSagaActivity>(
                    stepName,
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> Then(ThenActionDelegate<TSagaData> action)
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Then)),
                    action,
                    null,
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> Publish<TEvent, TCompensateEvent>()
            where TEvent : ISagaEvent, new()
            where TCompensateEvent : ISagaEvent, new()
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForSendActivity<TSagaData, TEvent, TCompensateEvent>(
                    null,
                    null,
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Publish), typeof(TEvent).Name),
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> Publish<TEvent, TCompensateEvent>(
            SendActionDelegate<TSagaData, TEvent> action, SendActionDelegate<TSagaData, TCompensateEvent> compensation)
            where TEvent : ISagaEvent, new()
            where TCompensateEvent : ISagaEvent, new()
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForSendActivity<TSagaData, TEvent, TCompensateEvent>(
                    action,
                    compensation,
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Publish), typeof(TEvent).Name),
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> Publish<TEvent>()
            where TEvent : ISagaEvent, new()
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForSendActivity<TSagaData, TEvent, EmptyEvent>(
                    null,
                    null,
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Publish), typeof(TEvent).Name),
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> Publish<TEvent>(SendActionDelegate<TSagaData, TEvent> action)
            where TEvent : ISagaEvent, new()
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForSendActivity<TSagaData, TEvent, EmptyEvent>(
                    action,
                    null,
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Publish), typeof(TEvent).Name),
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> Then(string stepName, ThenActionDelegate<TSagaData> action)
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    stepName,
                    action,
                    null,
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> Then(ThenActionDelegate<TSagaData> action,
            ThenActionDelegate<TSagaData> compensation)
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(Then)),
                    action,
                    compensation,
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> Then(string stepName, ThenActionDelegate<TSagaData> action,
            ThenActionDelegate<TSagaData> compensation)
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    stepName,
                    action,
                    compensation,
                    false,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> ThenAsync<TSagaActivity>()
            where TSagaActivity : ISagaActivity<TSagaData>
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThen<TSagaData, TSagaActivity>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(ThenAsync), typeof(TSagaActivity).Name),
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> ThenAsync<TSagaActivity>(string stepName)
            where TSagaActivity : ISagaActivity<TSagaData>
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThen<TSagaData, TSagaActivity>(
                    stepName,
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> ThenAsync(ThenActionDelegate<TSagaData> action)
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(ThenAsync)),
                    action,
                    null,
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> ThenAsync(string stepName, ThenActionDelegate<TSagaData> action)
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    stepName,
                    action,
                    null,
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> ThenAsync(ThenActionDelegate<TSagaData> action,
            ThenActionDelegate<TSagaData> compensation)
        {
            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(ThenAsync)),
                    action,
                    compensation,
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> ThenAsync(string stepName, ThenActionDelegate<TSagaData> action,
            ThenActionDelegate<TSagaData> compensation)
        {
            builderState.UniqueNameGenerator.ThrowIfNotUnique(stepName);

            builderState.Model.FindActionForStateAndEvent(builderState.CurrentState, builderState.CurrentEvent).ChildSteps.AddStep(
                new SagaStepForThenInline<TSagaData>(
                    stepName,
                    action,
                    compensation,
                    true,
                    builderState.ParentStep));

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> TransitionTo<TState>() where TState : ISagaState
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

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderHandle<TSagaData, TEvent> When<TEvent>() where TEvent : ISagaEvent
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

        public ISagaBuilderHandle<TSagaData, TEvent> When<TEvent, TEventHandler>() where TEvent : ISagaEvent
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

        public ISagaBuilderHandle<TSagaData, TEvent> WhenAsync<TEvent, TEventHandler>() where TEvent : ISagaEvent
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
        public ISagaBuilderThen<TSagaData> If<TSagaCondition>(Action<ISagaBuilderThen<TSagaData>> builderAction)
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

            SagaBuilder<TSagaData> childBuilder = new SagaBuilder<TSagaData>(childBuildState);
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

            return new SagaBuilder<TSagaData>(builderState);
        }
        public ISagaBuilderThen<TSagaData> If(IfFuncDelegate<TSagaData> condition, Action<ISagaBuilderThen<TSagaData>> builderAction)
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

            SagaBuilder<TSagaData> childBuilder = new SagaBuilder<TSagaData>(childBuildState);
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

            return new SagaBuilder<TSagaData>(builderState);
        }
        public ISagaBuilderThen<TSagaData> ElseIf(IfFuncDelegate<TSagaData> condition, Action<ISagaBuilderThen<TSagaData>> builderAction)
        {
            SagaAction currentAction = builderState.CurrentAction;

            SagaStepForElseIfInline<TSagaData> parentStep = new SagaStepForElseIfInline<TSagaData>(
                builderState.UniqueNameGenerator.Generate(builderState.CurrentState, nameof(If)),
                condition,
                null,
                builderState.ParentStep);

            SagaBuilderState childBuildState = new SagaBuilderState(
                null, null, new SagaModel(typeof(TSagaData)),
                builderState.ServiceProvider,
                builderState.UniqueNameGenerator,
                parentStep);

            SagaBuilder<TSagaData> childBuilder = new SagaBuilder<TSagaData>(childBuildState);
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

            return new SagaBuilder<TSagaData>(builderState);
        }
        public ISagaBuilderThen<TSagaData> Else(Action<ISagaBuilderThen<TSagaData>> builderAction)
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

            SagaBuilder<TSagaData> childBuilder = new SagaBuilder<TSagaData>(childBuildState);
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

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilderThen<TSagaData> Do(Action<ISagaBuilderThen<TSagaData>> builderAction)
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

            SagaBuilder<TSagaData> childBuilder = new SagaBuilder<TSagaData>(childBuildState);
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

            return new SagaBuilder<TSagaData>(builderState);
        }

        public ISagaBuilder<TSagaData> Settings(Action<ISagaSettingsBuilder> settingsBuilder)
        {
            ISagaSettingsBuilder sagaSettingsBuilder = new SagaSettingsBuilder(builderState.Model);
            settingsBuilder(sagaSettingsBuilder);
            return new SagaBuilder<TSagaData>(builderState);
        }
    }
}
