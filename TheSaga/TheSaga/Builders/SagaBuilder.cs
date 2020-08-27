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
        ISagaBuilderThen<TSagaData>,
        ISagaBuilderWhen<TSagaData>
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

        public ISagaBuilderThen<TSagaData> After(TimeSpan time)
        {
            throw new NotImplementedException();
        }

        public ISagaModel<TSagaData> Build()
        {
            return model;
        }

        public ISagaBuilderWhen<TSagaData> During<TState>()
            where TState : IState
        {
            currentState = typeof(TState).Name;
            currentEvent = null;
            return this;
        }

        public ISagaBuilder<TSagaData> Finish()
        {
            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                  new SagaStepForInlineAction<TSagaData>(
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

        public ISagaBuilderThen<TSagaData> Start<TEvent>()
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

        public ISagaBuilderThen<TSagaData> Start<TEvent, TEventHandler>()
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

        public ISagaBuilderThen<TSagaData> Start<TEvent>(string stepName)
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

        public ISagaBuilderThen<TSagaData> Start<TEvent, TEventHandler>(string stepName)
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

        public ISagaBuilderThen<TSagaData> StartAsync<TEvent, TEventHandler>()
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

        public ISagaBuilderThen<TSagaData> StartAsync<TEvent, TEventHandler>(string stepName)
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

        public ISagaBuilderThen<TSagaData> Then<TSagaActivity>()
            where TSagaActivity : ISagaActivity<TSagaData>
        {
            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForActivity<TSagaData, TSagaActivity>(
                    uniqueNameGenerator.Generate(currentState, nameof(Then), typeof(TSagaActivity).Name),
                    serviceProvider,
                    false));

            return this;
        }

        public ISagaBuilderThen<TSagaData> Then<TSagaActivity>(String stepName) where TSagaActivity : ISagaActivity<TSagaData>
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForActivity<TSagaData, TSagaActivity>(
                    stepName,
                    serviceProvider,
                    false));

            return this;
        }

        public ISagaBuilderThen<TSagaData> Then(ThenActionDelegate<TSagaData> action)
        {
            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    uniqueNameGenerator.Generate(currentState, nameof(Then)),
                    action,
                    null,
                    false));

            return this;
        }

        public ISagaBuilderThen<TSagaData> Then(String stepName, ThenActionDelegate<TSagaData> action)
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    stepName,
                    action,
                    null,
                    false));

            return this;
        }

        public ISagaBuilderThen<TSagaData> Then(ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation)
        {
            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    uniqueNameGenerator.Generate(currentState, nameof(Then)),
                    action,
                    compensation,
                    false));

            return this;
        }

        public ISagaBuilderThen<TSagaData> Then(String stepName, ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation)
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    stepName,
                    action,
                    compensation,
                    false));

            return this;
        }

        public ISagaBuilderThen<TSagaData> ThenAsync<TSagaActivity>()
                            where TSagaActivity : ISagaActivity<TSagaData>
        {
            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForActivity<TSagaData, TSagaActivity>(
                    uniqueNameGenerator.Generate(currentState, nameof(ThenAsync), typeof(TSagaActivity).Name),
                    serviceProvider,
                    true));

            return this;
        }

        public ISagaBuilderThen<TSagaData> ThenAsync<TSagaActivity>(String stepName) where TSagaActivity : ISagaActivity<TSagaData>
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForActivity<TSagaData, TSagaActivity>(
                    stepName,
                    serviceProvider,
                    true));

            return this;
        }

        public ISagaBuilderThen<TSagaData> ThenAsync(ThenActionDelegate<TSagaData> action)
        {
            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                uniqueNameGenerator.Generate(currentState, nameof(ThenAsync)),
                    action,
                    null,
                    true));

            return this;
        }

        public ISagaBuilderThen<TSagaData> ThenAsync(String stepName, ThenActionDelegate<TSagaData> action)
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    stepName,
                    action,
                    null,
                    true));

            return this;
        }

        public ISagaBuilderThen<TSagaData> ThenAsync(ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation)
        {
            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                uniqueNameGenerator.Generate(currentState, nameof(ThenAsync)),
                    action,
                    compensation,
                    true));

            return this;
        }

        public ISagaBuilderThen<TSagaData> ThenAsync(string stepName, ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation)
        {
            uniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    stepName,
                    action,
                    compensation,
                    true));

            return this;
        }

        public ISagaBuilderWhen<TSagaData> TransitionTo<TState>() where TState : IState
        {
            model.FindActionForStateAndEvent(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
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

        public ISagaBuilderThen<TSagaData> When<TEvent>() where TEvent : IEvent
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

        public ISagaBuilderThen<TSagaData> When<TEvent, TEventHandler>() where TEvent : IEvent
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

        public ISagaBuilderThen<TSagaData> WhenAsync<TEvent, TEventHandler>() where TEvent : IEvent
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