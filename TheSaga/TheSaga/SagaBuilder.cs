using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using TheSaga.Model;

namespace TheSaga
{
    public class SagaBuilder<TSagaState>
            where TSagaState : ISagaState
    {
        SagaModel<TSagaState> model;

        Type currentEvent;

        Type currentState;

        public SagaBuilder()
        {
            model = new SagaModel<TSagaState>();

            //var i = (TSagaType)Activator.CreateInstance<TSagaType>();
            //i.Define(this);
        }

        public SagaBuilder<TSagaState> After(TimeSpan time)
        {
            model.Actions.GetDuring(currentState, currentEvent).Steps.Add(new SagaStep<TSagaState>()
            {
                Action = ctx => Task.Delay(time)
            });
            return this;
        }

        public SagaModel<TSagaState> Build()
        {
            return model;
        }

        public SagaBuilder<TSagaState> During<TState_>()
            where TState_ : IState
        {
            currentState = typeof(TState_);
            currentEvent = null;
            return this;
        }

        public SagaBuilder<TSagaState> Start<TEvent>()
            where TEvent : IEvent
        {
            currentState = null;
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaSteps<TSagaState>()
            {
                State = currentState,
                Event = typeof(TEvent)
            });
            return this;
        }

        public SagaBuilder<TSagaState> Then<TSagaActivity>() where TSagaActivity : ISagaActivity<TSagaState>
        {
            model.Actions.GetDuring(currentState, currentEvent).Steps.Add(new SagaStep<TSagaState>()
            {
                Activity = typeof(TSagaActivity)
            });
            return this;
        }

        public SagaBuilder<TSagaState> Then(ThenFunction<TSagaState> action)
        {
            model.Actions.GetDuring(currentState, currentEvent).Steps.Add(new SagaStep<TSagaState>()
            {
                Action = action
            });
            return this;
        }

        public SagaBuilder<TSagaState> TransitionTo<TState>() where TState : IState
        {
            model.Actions.GetDuring(currentState, currentEvent).Steps.Add(new SagaStep<TSagaState>()
            {
                Action = ctx => { ctx.Data.CurrentState = currentState.Name; return Task.FromResult(0); }
            });
            return this;
        }

        public SagaBuilder<TSagaState> When<TEvent>() where TEvent : IEvent
        {
            currentEvent = typeof(TEvent);
            model.Actions.Add(new SagaSteps<TSagaState>()
            {
                State = currentState,
                Event = currentEvent
            });
            return this;
        }
    }

    public delegate Task ThenFunction<TState>(IContext<TState> context) where TState : ISagaState;
    public interface IContext<TState> where TState : ISagaState
    {
        TState Data { get; }
    }
}