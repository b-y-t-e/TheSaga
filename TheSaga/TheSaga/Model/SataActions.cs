using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSaga.Model
{
    public class SataActions<TSagaState> : List<SagaSteps<TSagaState>> 
        where TSagaState : ISagaState
    {
        public IList<SagaSteps<TSagaState>> GetStarts() =>
            this.Where(s => s.State == null).ToArray();

        public IList<SagaSteps<TSagaState>> GetDuring<TState>() where TState : IState =>
            this.Where(s => s.State == typeof(TState)).ToArray();

        public IList<SagaSteps<TSagaState>> GetDuring<TState, TEvent>() where TState : IState where TEvent : IEvent =>
            this.Where(s => s.State == typeof(TState) && s.Event == typeof(TEvent)).ToArray();

        public SagaSteps<TSagaState> GetDuring(Type stateType, Type eventType) =>
            this.FirstOrDefault(s => s.State == stateType && s.Event == eventType);

        public SataActions()
        {

        }
    }
    public class SagaSteps<TSagaState> where TSagaState : ISagaState
    {
        public Type State;

        public Type Event;

        public List<SagaStep<TSagaState>> Steps =
            new List<SagaStep<TSagaState>>();
    }

    public class SagaStep <TSagaState> where TSagaState : ISagaState
    {
        public ThenFunction<TSagaState> Action;
    }
}