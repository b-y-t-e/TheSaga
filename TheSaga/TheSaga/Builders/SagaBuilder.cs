using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Models;
using TheSaga.Models.Actions;
using TheSaga.Models.Steps;
using TheSaga.SagaModels;
using TheSaga.States;
using TheSaga.Utils;

namespace TheSaga.Builders
{
    internal class SagaBuilder<TSagaData, TEvent> : SagaBuilder<TSagaData>,
        ISagaBuilder<TSagaData>,
        ISagaBuilderThen<TSagaData>,
        ISagaBuilderWhen<TSagaData>,
        ISagaBuilderHandle<TSagaData, TEvent>
        where TSagaData : ISagaData
        where TEvent : IEvent
    {
        public SagaBuilder(IServiceProvider serviceProvider) :
            base(serviceProvider)
        {
        }

        public SagaBuilder(SagaBuilderState<TSagaData> sagaBuilderState) :
            base(sagaBuilderState)
        {
        }

        public ISagaBuilderThen<TSagaData> HandleBy<TEventHandler>()
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            if (s.CurrentEvent == null)
                throw new Exception($"{nameof(HandleBy)} must be defined after {nameof(When)} / {nameof(WhenAsync)}");

            var action = s.Model.Actions.FindAction(s.CurrentState, s.CurrentEvent);
            action.Steps.Clear();
            action.Steps.Add(new SagaStepForEventHandler<TSagaData, TEventHandler>(
                        s.UniqueNameGenerator.Generate(s.CurrentState, nameof(HandleBy), s.CurrentEvent.GetType().Name, typeof(TEventHandler).Name),
                        s.ServiceProvider,
                        false));

            return new SagaBuilder<TSagaData>(s);
        }
    }

    internal class SagaBuilder<TSagaData> :
        ISagaBuilder<TSagaData>,
        ISagaBuilderThen<TSagaData>,
        ISagaBuilderWhen<TSagaData>
        where TSagaData : ISagaData
    {
        protected SagaBuilderState<TSagaData> s;

        public SagaBuilder(IServiceProvider serviceProvider)
        {
            s = new SagaBuilderState<TSagaData>(
                null,
                null,
                new SagaModel<TSagaData>(),
                serviceProvider,
                new UniqueNameGenerator());
        }

        public SagaBuilder(SagaBuilderState<TSagaData> sagaBuilderState)
        {
            this.s = sagaBuilderState;
        }

        public ISagaBuilderThen<TSagaData> After(TimeSpan time)
        {
            throw new NotImplementedException();
        }

        public ISagaModel<TSagaData> Build()
        {
            if (String.IsNullOrEmpty(s.Model.Name))
                throw new InvalidSagaModelNameException();

            return s.Model;
        }

        public ISagaBuilderWhen<TSagaData> During<TState>()
            where TState : IState
        {
            s.CurrentState = typeof(TState).Name;
            s.CurrentEvent = null;
            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilder<TSagaData> Finish()
        {
            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                  new SagaStepForInlineAction<TSagaData>(
                      s.UniqueNameGenerator.Generate(s.CurrentState, nameof(Finish)),
                      ctx =>
                      {
                          ctx.State.CurrentState = new SagaFinishState().GetStateName();
                          ctx.State.CurrentStep = null;
                          ctx.State.IsCompensating = false;
                          return Task.CompletedTask;
                      },
                      null,
                      false));
            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilder<TSagaData> Name(string name)
        {
            s.Model.Name = name;
            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Start<TEvent>()
                            where TEvent : IEvent
        {
            s.CurrentState = new SagaStartState().GetStateName();
            s.CurrentEvent = typeof(TEvent);
            s.Model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = s.CurrentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaEmptyStep(
                        s.UniqueNameGenerator.Generate(s.CurrentState, nameof(Start), typeof(TEvent).Name)                      )
                }
            });
            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Start<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            s.CurrentState = new SagaStartState().GetStateName();
            s.CurrentEvent = typeof(TEvent);
            s.Model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = s.CurrentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        s.UniqueNameGenerator.Generate(s.CurrentState, nameof(Start), typeof(TEvent).Name, typeof(TEventHandler).Name),
                        s.ServiceProvider,
                        false)
                }
            });
            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Start<TEvent>(string stepName)
                    where TEvent : IEvent
        {
            s.UniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            s.CurrentState = new SagaStartState().GetStateName();
            s.CurrentEvent = typeof(TEvent);
            s.Model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = s.CurrentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaEmptyStep(
                        stepName)
                }
            });
            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Start<TEvent, TEventHandler>(string stepName)
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            s.UniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            s.CurrentState = new SagaStartState().GetStateName();
            s.CurrentEvent = typeof(TEvent);
            s.Model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = s.CurrentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        stepName,
                        s.ServiceProvider,
                        false)
                }
            });
            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> StartAsync<TEvent, TEventHandler>()
                            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            s.CurrentState = new SagaStartState().GetStateName();
            s.CurrentEvent = typeof(TEvent);
            s.Model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = s.CurrentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        s.UniqueNameGenerator.Generate(s.CurrentState, nameof(StartAsync), typeof(TEvent).Name, typeof(TEventHandler).Name),
                        s.ServiceProvider,
                        true)
                }
            });
            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> StartAsync<TEvent, TEventHandler>(string stepName)
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            s.UniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            s.CurrentState = new SagaStartState().GetStateName();
            s.CurrentEvent = typeof(TEvent);
            s.Model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = s.CurrentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        stepName,
                        s.ServiceProvider,
                        true)
                }
            });
            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Then<TSagaActivity>()
            where TSagaActivity : ISagaActivity<TSagaData>
        {
            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForActivity<TSagaData, TSagaActivity>(
                    s.UniqueNameGenerator.Generate(s.CurrentState, nameof(Then), typeof(TSagaActivity).Name),
                    s.ServiceProvider,
                    false));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Then<TSagaActivity>(String stepName) where TSagaActivity : ISagaActivity<TSagaData>
        {
            s.UniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForActivity<TSagaData, TSagaActivity>(
                    stepName,
                    s.ServiceProvider,
                    false));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Then(ThenActionDelegate<TSagaData> action)
        {
            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    s.UniqueNameGenerator.Generate(s.CurrentState, nameof(Then)),
                    action,
                    null,
                    false));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Send<TEvent, TCompensateEvent>()
            where TEvent : IEvent, new()
            where TCompensateEvent : IEvent, new()
        {
            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForSendActivity<TSagaData, TEvent, TCompensateEvent>(
                    null,
                    null,
                    s.UniqueNameGenerator.Generate(s.CurrentState, nameof(Send), typeof(TEvent).Name),
                    s.ServiceProvider,
                    false));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Send<TEvent, TCompensateEvent>(SendActionDelegate<TSagaData, TEvent> action, SendActionDelegate<TSagaData, TCompensateEvent> compensation)
            where TEvent : IEvent, new()
            where TCompensateEvent : IEvent, new()
        {
            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForSendActivity<TSagaData, TEvent, TCompensateEvent>(
                    action,
                    compensation,
                    s.UniqueNameGenerator.Generate(s.CurrentState, nameof(Send), typeof(TEvent).Name),
                    s.ServiceProvider,
                    false));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Send<TEvent>()
            where TEvent : IEvent, new()
        {
            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForSendActivity<TSagaData, TEvent, EmptyEvent>(
                    null,
                    null,
                    s.UniqueNameGenerator.Generate(s.CurrentState, nameof(Send), typeof(TEvent).Name),
                    s.ServiceProvider,
                    false));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Send<TEvent>(SendActionDelegate<TSagaData, TEvent> action)
            where TEvent : IEvent, new()
        {
            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForSendActivity<TSagaData, TEvent, EmptyEvent>(
                    action,
                    null,
                    s.UniqueNameGenerator.Generate(s.CurrentState, nameof(Send), typeof(TEvent).Name),
                    s.ServiceProvider,
                    false));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Then(String stepName, ThenActionDelegate<TSagaData> action)
        {
            s.UniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    stepName,
                    action,
                    null,
                    false));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Then(ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation)
        {
            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    s.UniqueNameGenerator.Generate(s.CurrentState, nameof(Then)),
                    action,
                    compensation,
                    false));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> Then(String stepName, ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation)
        {
            s.UniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    stepName,
                    action,
                    compensation,
                    false));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> ThenAsync<TSagaActivity>()
                            where TSagaActivity : ISagaActivity<TSagaData>
        {
            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForActivity<TSagaData, TSagaActivity>(
                    s.UniqueNameGenerator.Generate(s.CurrentState, nameof(ThenAsync), typeof(TSagaActivity).Name),
                    s.ServiceProvider,
                    true));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> ThenAsync<TSagaActivity>(String stepName) where TSagaActivity : ISagaActivity<TSagaData>
        {
            s.UniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForActivity<TSagaData, TSagaActivity>(
                    stepName,
                    s.ServiceProvider,
                    true));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> ThenAsync(ThenActionDelegate<TSagaData> action)
        {
            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                s.UniqueNameGenerator.Generate(s.CurrentState, nameof(ThenAsync)),
                    action,
                    null,
                    true));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> ThenAsync(String stepName, ThenActionDelegate<TSagaData> action)
        {
            s.UniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    stepName,
                    action,
                    null,
                    true));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> ThenAsync(ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation)
        {
            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                s.UniqueNameGenerator.Generate(s.CurrentState, nameof(ThenAsync)),
                    action,
                    compensation,
                    true));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> ThenAsync(string stepName, ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation)
        {
            s.UniqueNameGenerator.
                ThrowIfNotUnique(stepName);

            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    stepName,
                    action,
                    compensation,
                    true));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderThen<TSagaData> TransitionTo<TState>() where TState : IState
        {
            s.Model.FindActionForStateAndEvent(s.CurrentState, s.CurrentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaData>(
                    s.UniqueNameGenerator.Generate(s.CurrentState, nameof(TransitionTo), typeof(TState).Name),
                    ctx =>
                    {
                        ctx.State.CurrentState = typeof(TState).Name;
                        return Task.CompletedTask;
                    },
                    ctx =>
                    {
                        var data = ctx.State.CurrentStepData();
                        ctx.State.CurrentState = data.StateName;
                        return Task.CompletedTask;
                    },
                    false));

            return new SagaBuilder<TSagaData>(s);
        }

        public ISagaBuilderHandle<TSagaData, TEvent> When<TEvent>() where TEvent : IEvent
        {
            s.CurrentEvent = typeof(TEvent);
            s.Model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = s.CurrentState,
                Event = s.CurrentEvent,
                Steps = new List<ISagaStep>
                {
                    new SagaEmptyStep(
                        s.UniqueNameGenerator.Generate(s.CurrentState, nameof(When), typeof(TEvent).Name))
                }
            });
            return new SagaBuilder<TSagaData, TEvent>(s);
        }

        public ISagaBuilderHandle<TSagaData, TEvent> When<TEvent, TEventHandler>() where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            s.CurrentEvent = typeof(TEvent);
            s.Model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = s.CurrentState,
                Event = s.CurrentEvent,
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        s.UniqueNameGenerator.Generate(s.CurrentState, nameof(When), typeof(TEvent).Name, typeof(TEventHandler).Name),
                        s.ServiceProvider,
                        false)
                }
            });
            return new SagaBuilder<TSagaData, TEvent>(s);
        }

        public ISagaBuilderHandle<TSagaData, TEvent> WhenAsync<TEvent, TEventHandler>() where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaData, TEvent>
        {
            s.CurrentEvent = typeof(TEvent);
            s.Model.Actions.Add(new SagaAction<TSagaData>()
            {
                State = s.CurrentState,
                Event = s.CurrentEvent,
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaData, TEventHandler, TEvent>(
                        s.UniqueNameGenerator.Generate(s.CurrentState, nameof(WhenAsync), typeof(TEvent).Name, typeof(TEventHandler).Name),
                        s.ServiceProvider,
                        true)
                }
            });
            return new SagaBuilder<TSagaData, TEvent>(s);
        }
    }
}