using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
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
            throw new NotImplementedException();
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
                State = null,
                Event = typeof(TEvent)
            });
            return this;
        }

        public SagaBuilder<TSagaState> Then(Type activityType)
        {
            throw new NotImplementedException();
        }

        public SagaBuilder<TSagaState> Then(Type activityType, Type compensateType)
        {
            throw new NotImplementedException();
        }

        public SagaBuilder<TSagaState> Then(ThenFunction<TSagaState> action)
        {
            throw new NotImplementedException();
        }

        public SagaBuilder<TSagaState> TransitionTo<TState>() where TState : IState
        {
            model.Actions.GetDuring(currentState, currentEvent).Steps.Add(new SagaStep<TSagaState>()
            {
                Action = ctx => ctx.Data.CurrentState = currentState.Name
            });
            return this;
        }

        public SagaBuilder<TSagaState> When<TEvent>() where TEvent : IEvent
        {
            model.Actions.Add(new SagaSteps<TSagaState>()
            {
                State = currentEvent,
                Event = typeof(TEvent)
            });
            return this;
        }
    }

    public delegate void ThenFunction<TState>(IContext<TState> context) where TState : ISagaState;
    public interface IContext<TState> where TState : ISagaState
    {
        TState Data { get; }
    }
}