using System;
using TheSaga.Model;

namespace TheSaga
{
    public class SagaBuilder<TSagaType, TState> : ISagaBuilder<TSagaType, TState>
            where TSagaType : ISaga<TState>
            where TState : ISagaState
    {
        SagaModel model;

        public SagaBuilder()
        {
            this.model = new SagaModel();
            this.model.Init(typeof(TSagaType));

            var i = Activator.CreateInstance<TSagaType>();
            i = i;
        }

        public ISaga<TState> After(TimeSpan time)
        {
            throw new NotImplementedException();
        }

        public SagaModel Build()
        {
            throw new NotImplementedException();
        }

        public ISaga<TState> During(IState state)
        {
            throw new NotImplementedException();
        }

        public ISaga<TState> Start(IEvent @event)
        {
            throw new NotImplementedException();
        }

        public ISaga<TState> Then(Type activityType)
        {
            throw new NotImplementedException();
        }

        public ISaga<TState> Then(Type activityType, Type compensateType)
        {
            throw new NotImplementedException();
        }

        public ISaga<TState> Then(ThenFunction action)
        {
            throw new NotImplementedException();
        }

        public ISaga<TState> TransitionTo(IState state)
        {
            throw new NotImplementedException();
        }

        public ISaga<TState> When(IEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}