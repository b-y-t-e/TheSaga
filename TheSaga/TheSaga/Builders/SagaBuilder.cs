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
    public class SagaBuilder<TSagaState> : ISagaBuilder<TSagaState> 
        where TSagaState : ISagaState
    {
        SagaModel<TSagaState> model;

        Type currentEvent;

        String currentState;

        public SagaBuilder()
        {
            model = new SagaModel<TSagaState>();
        }

        public SagaBuilder<TSagaState> After(TimeSpan time)
        {
            model.FindAction(currentState, currentEvent).Steps.Add(
                new SagaStepInlineAction<TSagaState>(
                    $"{currentState}_{nameof(After)}",
                    ctx => Task.Delay(time)));

            return this;
        }

        public SagaModel<TSagaState> Build()
        {
            return model;
        }

        public SagaBuilder<TSagaState> During<TState>()
            where TState : IState
        {
            currentState = typeof(TState).Name;
            currentEvent = null;
            return this;
        }

        public SagaBuilder<TSagaState> Start<TEvent>()
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

        public SagaBuilder<TSagaState> Start<TEvent, TEventHandler>()
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
                    new SagaStepEventHandler<TSagaState, TEventHandler, TEvent>(
                        $"{currentState}_{nameof(Start)}_{typeof(TEvent).Name}")
                }
            });
            return this;
        }

        public SagaBuilder<TSagaState> Then<TSagaActivity>()
            where TSagaActivity : ISagaActivity<TSagaState>
        {
            return Then<TSagaActivity>(
                $"{currentState}_{nameof(Then)}_{typeof(TSagaActivity).Name}");
        }

        public SagaBuilder<TSagaState> Then<TSagaActivity>(String stepName) where TSagaActivity : ISagaActivity<TSagaState>
        {
            model.FindAction(currentState, currentEvent).Steps.Add(
                new SagaStepActivity<TSagaState, TSagaActivity>(
                    stepName));

            return this;
        }

        public SagaBuilder<TSagaState> Then(ThenActionDelegate<TSagaState> action)
        {
            return Then(
                $"{currentState}_{nameof(Then)}_action", action);
        }

        public SagaBuilder<TSagaState> Then(String stepName, ThenActionDelegate<TSagaState> action)
        {
            model.FindAction(currentState, currentEvent).Steps.Add(
                new SagaStepInlineAction<TSagaState>(
                    stepName,
                    action));

            return this;
        }

        public SagaBuilder<TSagaState> TransitionTo<TState>() where TState : IState
        {
            model.FindAction(currentState, currentEvent).Steps.Add(
                new SagaStepInlineAction<TSagaState>(
                    $"{currentState}_{nameof(TransitionTo)}_{typeof(TState).Name}",
                    ctx =>
                    {
                        ctx.State.CurrentState = typeof(TState).Name;
                        return Task.CompletedTask;
                    }));
            return this;
        }

        public SagaBuilder<TSagaState> When<TEvent>() where TEvent : IEvent
        {
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
            {
                State = currentState,
                Event = currentEvent
            });
            return this;
        }

        public SagaBuilder<TSagaState> When<TEvent, TEventHandler>() where TEvent : IEvent
            where TEventHandler : IEventHandler<TSagaState, TEvent>
        {
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaAction<TSagaState>()
            {
                State = currentState,
                Event = currentEvent,
                Steps = new List<ISagaStep>
                {
                    new SagaStepEventHandler<TSagaState, TEventHandler, TEvent>(
                        $"{currentState}_{nameof(When)}_{typeof(TEvent).Name}")
                }
            }); 
            return this;
        }
    }
}