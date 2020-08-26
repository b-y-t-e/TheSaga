using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;
using TheSaga.States;
using TheSaga.Utils;

namespace TheSaga.Builders
{
    public class SagaBuilder<TSagaData> :
        ISagaBuilder<TSagaData>,
        ISagaBuilderDuringState<TSagaData>,
        ISagaBuilderState<TSagaData>
        where TSagaData : ISagaData
    {
        private Type currentEvent;
        private String currentState;
        private SagaModel<TSagaData> model;
        private IServiceProvider serviceProvider;
        private UniqueNameGenerator uniqueNameGenerator;

        public SagaBuilder(IServiceProvider serviceProvider)
        {
            model = new SagaModel<TSagaData>();
            this.serviceProvider = serviceProvider;
            this.uniqueNameGenerator = new UniqueNameGenerator();
        }

        public ISagaBuilderState<TSagaData> After(TimeSpan time)
        {
            throw new NotImplementedException();
        }

        public ISagaModel<TSagaData> Build()
        {
            return model;
        }

        public ISagaBuilderDuringState<TSagaData> During<TState>()
            where TState : IState
        {
            currentState = typeof(TState).Name;
            currentEvent = null;
            return this;
        }

        public ISagaBuilder<TSagaData> Finish()
        {
            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                  new SagaStepForInlineAction<TSagaData>(
                      uniqueNameGenerator.Generate(currentState, nameof(Finish)),
                      ctx =>
                      {
                          ctx.State.SagaState.CurrentState = new SagaFinishState().GetStateName();
                          ctx.State.SagaState.CurrentStep = null;
                          ctx.State.SagaState.IsCompensating = false;
                          return Task.CompletedTask;
                      },
                      null,
                      false));
            return this;
        }

        public ISagaBuilderState<TSagaData> Start<TEvent>()
                    where TEvent : IEvent
        {
            currentState = new SagaStartState().GetStateName();
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = currentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaEmptyStep(
                        uniqueNameGenerator.Generate(currentState, nameof(Start), typeof(TEvent).Name)                      )
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaData> Start<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            currentState = new SagaStartState().GetStateName();
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = currentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        uniqueNameGenerator.Generate(currentState, nameof(Start), typeof(TEvent).Name),
                        serviceProvider,
                        false)
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaData> Start<TEvent>(string stepName)
                    where TEvent : IEvent
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            currentState = new SagaStartState().GetStateName();
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = currentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaEmptyStep(
                        stepName)
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaData> Start<TEvent, TEventHandler>(string stepName)
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            currentState = new SagaStartState().GetStateName();
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = currentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        stepName,
                        serviceProvider,
                        false)
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaData> StartAsync<TEvent, TEventHandler>()
                            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            currentState = new SagaStartState().GetStateName();
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = currentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        uniqueNameGenerator.Generate(currentState, nameof(StartAsync), typeof(TEvent).Name),
                        serviceProvider,
                        true)
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaData> StartAsync<TEvent, TEventHandler>(string stepName)
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            currentState = new SagaStartState().GetStateName();
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = currentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        stepName,
                        serviceProvider,
                        true)
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaData> Then<TSagaActivity>()
            where TSagaActivity : ISagaActivity<TSagaData>
        {
            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForActivity<TSagaData, TSagaActivity>(
                    uniqueNameGenerator.Generate(currentState, nameof(Then), typeof(TSagaActivity).Name),
                    serviceProvider,
                    false));

            return this;
        }

        public ISagaBuilderState<TSagaData> Then<TSagaActivity>(String stepName) where TSagaActivity : ISagaActivity<TSagaData>
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForActivity<TSagaData, TSagaActivity>(
                    stepName,
                    serviceProvider,
                    false));

            return this;
        }

        public ISagaBuilderState<TSagaData> Then(ThenActionDelegate<TSagaData> action)
        {
            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    uniqueNameGenerator.Generate(currentState, nameof(Then)),
                    action,
                    null,
                    false));

            return this;
        }

        public ISagaBuilderState<TSagaData> Then(String stepName, ThenActionDelegate<TSagaData> action)
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    stepName,
                    action,
                    null,
                    false));

            return this;
        }

        public ISagaBuilderState<TSagaData> Then(ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation)
        {
            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    uniqueNameGenerator.Generate(currentState, nameof(Then)),
                    action,
                    compensation,
                    false));

            return this;
        }

        public ISagaBuilderState<TSagaData> Then(String stepName, ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation)
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    stepName,
                    action,
                    compensation,
                    false));

            return this;
        }

        public ISagaBuilderState<TSagaData> ThenAsync<TSagaActivity>()
                            where TSagaActivity : ISagaActivity<TSagaData>
        {
            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForActivity<TSagaData, TSagaActivity>(
                    uniqueNameGenerator.Generate(currentState, nameof(ThenAsync), typeof(TSagaActivity).Name),
                    serviceProvider,
                    true));

            return this;
        }

        public ISagaBuilderState<TSagaData> ThenAsync<TSagaActivity>(String stepName) where TSagaActivity : ISagaActivity<TSagaData>
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForActivity<TSagaData, TSagaActivity>(
                    stepName,
                    serviceProvider,
                    true));

            return this;
        }

        public ISagaBuilderState<TSagaData> ThenAsync(ThenActionDelegate<TSagaData> action)
        {
            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                uniqueNameGenerator.Generate(currentState, nameof(ThenAsync)),
                    action,
                    null,
                    true));

            return this;
        }

        public ISagaBuilderState<TSagaData> ThenAsync(String stepName, ThenActionDelegate<TSagaData> action)
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    stepName,
                    action,
                    null,
                    true));

            return this;
        }

        public ISagaBuilderState<TSagaData> ThenAsync(ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation)
        {
            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                uniqueNameGenerator.Generate(currentState, nameof(ThenAsync)),
                    action,
                    compensation,
                    true));

            return this;
        }

        public ISagaBuilderState<TSagaData> ThenAsync(string stepName, ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation)
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    stepName,
                    action,
                    compensation,
                    true));

            return this;
        }

        public ISagaBuilder<TSagaData> TransitionTo<TState>() where TState : IState
        {
            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    uniqueNameGenerator.Generate(currentState, nameof(TransitionTo), typeof(TState).Name),
                    ctx =>
                    {
                        ctx.State.SagaState.CurrentState = typeof(TState).Name;
                        return Task.CompletedTask;
                    },
                    null,
                    false));

            return this;
        }

        public ISagaBuilderState<TSagaData> When<TEvent>() where TEvent : IEvent
        {
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = currentState,
                Event = currentEvent,
                Steps = new List<ISagaStep>
                {
                    new SagaEmptyStep(
                        uniqueNameGenerator.Generate(currentState, nameof(When), typeof(TEvent).Name))
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaData> When<TEvent, TEventHandler>() where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = currentState,
                Event = currentEvent,
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        uniqueNameGenerator.Generate(currentState, nameof(When), typeof(TEvent).Name),
                        serviceProvider,
                        false)
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaData> WhenAsync<TEvent, TEventHandler>() where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = currentState,
                Event = currentEvent,
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        uniqueNameGenerator.Generate(currentState, nameof(WhenAsync), typeof(TEvent).Name),
                        serviceProvider,
                        true)
                }
            });
            return this;
        }
    }
}