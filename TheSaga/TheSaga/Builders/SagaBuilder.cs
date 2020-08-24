using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Interfaces;
using TheSaga.Models;
using TheSaga.States;
using TheSaga.States.Actions;

namespace TheSaga.Builders
{
    public class SagaBuilder<TSagaState> : 
        ISagaBuilder<TSagaState>,
        ISagaBuilderDuringState<TSagaState>,
        ISagaBuilderState<TSagaState>
        where TSagaState : ISagaState
    {
        IServiceProvider serviceProvider;

        SagaModel<TSagaState> model;

        Type currentEvent;

        String currentState;

        public SagaBuilder(IServiceProvider serviceProvider)
        {
            model = new SagaModel<TSagaState>();
            this.serviceProvider = serviceProvider;
        }

        public ISagaBuilderState<TSagaState> After(TimeSpan time)
        {
            model.FindAction(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaState>(
                    $"{currentState}_{nameof(After)}",
                    ctx => Task.Delay(time)));

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

        public ISagaBuilderState<TSagaState> Start<TEvent>()
            where TEvent : IEvent
        {
            currentState = null;
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
            {
                State = currentState,
                Event = typeof(TEvent)
            });
            return this;
        }

        public ISagaBuilderState<TSagaState> Start<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaState, TEvent>
        {
            currentState = null;
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
            {
                State = currentState,
                Event = typeof(TEvent),
                Steps = new List<ISagaStep>
                {
                    new SagaStepForEventHandler<TSagaState, TEventHandler, TEvent>(
                        $"{currentState}_{nameof(Start)}_{typeof(TEvent).Name}",
                        serviceProvider)
                }
            });
            return this;
        }

        public ISagaBuilderState<TSagaState> Then<TSagaActivity>()
            where TSagaActivity : ISagaActivity<TSagaState>
        {
            return Then<TSagaActivity>(
                $"{currentState}_{nameof(Then)}_{typeof(TSagaActivity).Name}");
        }

        public ISagaBuilderState<TSagaState> Then<TSagaActivity>(String stepName) where TSagaActivity : ISagaActivity<TSagaState>
        {
            model.FindAction(currentState, currentEvent).Steps.Add(
                new SagaStepForActivity<TSagaState, TSagaActivity>(
                    stepName,
                        serviceProvider));

            return this;
        }

        public ISagaBuilderState<TSagaState> Then(ThenActionDelegate<TSagaState> action)
        {
            return Then(
                $"{currentState}_{nameof(Then)}_action", action);
        }

        public ISagaBuilderState<TSagaState> Then(String stepName, ThenActionDelegate<TSagaState> action)
        {
            model.FindAction(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaState>(
                    stepName,
                    action));

            return this;
        }

        public ISagaBuilderState<TSagaState> TransitionTo<TState>() where TState : IState
        {
            model.FindAction(currentState, currentEvent).Steps.Add(
                new SagaStepForInlineAction<TSagaState>(
                    $"{currentState}_{nameof(TransitionTo)}_{typeof(TState).Name}",
                    ctx =>
                    {
                        ctx.State.CurrentState = typeof(TState).Name;
                        return Task.CompletedTask;
                    }));
            return this;
        }

        public ISagaBuilderState<TSagaState> When<TEvent>() where TEvent : IEvent
        {
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
            {
                State = currentState,
                Event = currentEvent
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
                        $"{currentState}_{nameof(When)}_{typeof(TEvent).Name}",
                        serviceProvider)
                }
            });
            return this;
        }
    }
}