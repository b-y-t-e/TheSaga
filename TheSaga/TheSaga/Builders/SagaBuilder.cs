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
    public class SagaBuilder<TSagaState> :
        ISagaBuilder<TSagaState>,
        ISagaBuilderDuringState<TSagaState>,
        ISagaBuilderState<TSagaState>
        where TSagaState : ISagaState
    {
        private Type currentEvent;
        private String currentState;
        private SagaModel<TSagaState> model;
        private IServiceProvider serviceProvider;
        private UniqueNameGenerator uniqueNameGenerator;

        public SagaBuilder(IServiceProvider serviceProvider)
        {
            model = new SagaModel<TSagaState>();
            this.serviceProvider = serviceProvider;
            this.uniqueNameGenerator = new UniqueNameGenerator();
        }

        public ISagaBuilderState<TSagaState> After(TimeSpan time)
        {
            throw new NotImplementedException();

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaState>(
                    uniqueNameGenerator.Generate(currentState, nameof(After)),
                    ctx => Task.Delay(time),
                    null,
                    true));

            return this;
        }

        public ISagaModel<TSagaState> Build()
        {
            return model;
        }

        public ISagaBuilderDuringState<TSagaState> During<TState>()
            where TState : IState
        {
            currentState = typeof(TState).Name;
            currentEvent = null;
            return this;
        }

        public ISagaBuilder<TSagaState> Finish()
        {
            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                  new SagaStepForInlineAction<TSagaState>(
                      uniqueNameGenerator.Generate(currentState, nameof(Finish)),
                      ctx =>
                      {
                          ctx.State.CurrentState = new SagaFinishState().GetStateName();
                          ctx.State.CurrentStep = null;
                          ctx.State.IsCompensating = false;
                          return Task.CompletedTask;
                      },
                      null,
                      false));
            return this;
        }

        public ISagaBuilderState<TSagaState> Start<TEvent>()
                    where TEvent : IEvent
        {
            currentState = new SagaStartState().GetStateName();
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
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

        public ISagaBuilderState<TSagaState> Start<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaState, TEvent>
        {
            currentState = new SagaStartState().GetStateName();
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
            {
                State = currentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaState, TEventHandler, TEvent>(
                        uniqueNameGenerator.Generate(currentState, nameof(Start), typeof(TEvent).Name),
                        serviceProvider,
                        false)
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaState> StartAsync<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaState, TEvent>
        {
            currentState = new SagaStartState().GetStateName();
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
            {
                State = currentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaState, TEventHandler, TEvent>(
                        uniqueNameGenerator.Generate(currentState, nameof(StartAsync), typeof(TEvent).Name),
                        serviceProvider,
                        true)
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaState> Start<TEvent>(string stepName)
                    where TEvent : IEvent
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            currentState = new SagaStartState().GetStateName();
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
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

        public ISagaBuilderState<TSagaState> Start<TEvent, TEventHandler>(string stepName)
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaState, TEvent>
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            currentState = new SagaStartState().GetStateName();
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
            {
                State = currentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaState, TEventHandler, TEvent>(
                        stepName,
                        serviceProvider,
                        false)
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaState> StartAsync<TEvent, TEventHandler>(string stepName)
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaState, TEvent>
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            currentState = new SagaStartState().GetStateName();
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
            {
                State = currentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaState, TEventHandler, TEvent>(
                        stepName,
                        serviceProvider,
                        true)
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaState> Then<TSagaActivity>()
            where TSagaActivity : ISagaActivity<TSagaState>
        {
            return Then<TSagaActivity>(
                uniqueNameGenerator.Generate(currentState, nameof(Then), typeof(TSagaActivity).Name));
        }

        public ISagaBuilderState<TSagaState> Then<TSagaActivity>(String stepName) where TSagaActivity : ISagaActivity<TSagaState>
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForActivity<TSagaState, TSagaActivity>(
                    stepName,
                    serviceProvider,
                    false));

            return this;
        }

        public ISagaBuilderState<TSagaState> Then(ThenActionDelegate<TSagaState> action)
        {
            return Then(
                uniqueNameGenerator.Generate(currentState, nameof(Then)),
                action);
        }

        public ISagaBuilderState<TSagaState> Then(String stepName, ThenActionDelegate<TSagaState> action)
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaState>(
                    stepName,
                    action,
                    null,
                    false));

            return this;
        }
        public ISagaBuilderState<TSagaState> Then(ThenActionDelegate<TSagaState> action, ThenActionDelegate<TSagaState> compensation)
        {
            return Then(
                uniqueNameGenerator.Generate(currentState, nameof(Then)),
                action,
                compensation);
        }

        public ISagaBuilderState<TSagaState> Then(String stepName, ThenActionDelegate<TSagaState> action, ThenActionDelegate<TSagaState> compensation)
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaState>(
                    stepName,
                    action,
                    compensation,
                    false));

            return this;
        }

        public ISagaBuilderState<TSagaState> ThenAsync<TSagaActivity>()
                            where TSagaActivity : ISagaActivity<TSagaState>
        {
            return ThenAsync<TSagaActivity>(
                uniqueNameGenerator.Generate(currentState, nameof(ThenAsync), typeof(TSagaActivity).Name));
        }

        public ISagaBuilderState<TSagaState> ThenAsync<TSagaActivity>(String stepName) where TSagaActivity : ISagaActivity<TSagaState>
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForActivity<TSagaState, TSagaActivity>(
                    stepName,
                    serviceProvider,
                    true));

            return this;
        }

        public ISagaBuilderState<TSagaState> ThenAsync(ThenActionDelegate<TSagaState> action)
        {
            return ThenAsync(
                uniqueNameGenerator.Generate(currentState, nameof(ThenAsync)),
                action);
        }

        public ISagaBuilderState<TSagaState> ThenAsync(String stepName, ThenActionDelegate<TSagaState> action)
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaState>(
                    stepName,
                    action,
                    null,
                    true));

            return this;
        }

        public ISagaBuilderState<TSagaState> ThenAsync(ThenActionDelegate<TSagaState> action, ThenActionDelegate<TSagaState> compensation)
        {
            return ThenAsync(
                uniqueNameGenerator.Generate(currentState, nameof(ThenAsync)),
                action,
                compensation);
        }

        public ISagaBuilderState<TSagaState> ThenAsync(string stepName, ThenActionDelegate<TSagaState> action, ThenActionDelegate<TSagaState> compensation)
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaState>(
                    stepName,
                    action,
                    compensation,
                    true));

            return this;
        }

        public ISagaBuilder<TSagaState> TransitionTo<TState>() where TState : IState
        {
            model.FindActionOrCreateForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaState>(
                    uniqueNameGenerator.Generate(currentState, nameof(TransitionTo), typeof(TState).Name),
                    ctx =>
                    {
                        ctx.State.CurrentState = typeof(TState).Name;
                        return Task.CompletedTask;
                    },
                    null,
                    false));

            return this;
        }

        public ISagaBuilderState<TSagaState> When<TEvent>() where TEvent : IEvent
        {
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
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

        public ISagaBuilderState<TSagaState> When<TEvent, TEventHandler>() where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaState, TEvent>
        {
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
            {
                State = currentState,
                Event = currentEvent,
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaState, TEventHandler, TEvent>(
                        uniqueNameGenerator.Generate(currentState, nameof(When), typeof(TEvent).Name),
                        serviceProvider,
                        false)
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaState> WhenAsync<TEvent, TEventHandler>() where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaState, TEvent>
        {
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
            {
                State = currentState,
                Event = currentEvent,
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaState, TEventHandler, TEvent>(
                        uniqueNameGenerator.Generate(currentState, nameof(WhenAsync), typeof(TEvent).Name),
                        serviceProvider,
                        true)
                }
            });
            return this;
        }
    }
}