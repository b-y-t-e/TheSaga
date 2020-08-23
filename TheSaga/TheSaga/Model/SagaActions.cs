using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSaga.Model
{
    public class SagaActions<TSagaState> : List<SagaAction<TSagaState>>
        where TSagaState : ISagaState
    {
        public IList<SagaAction<TSagaState>> GetStarts() =>
            this.Where(s => s.State == null).ToArray();

        public SagaAction<TSagaState> GetDuring(Type stateType, Type eventType)
        {
            var steps = this.FirstOrDefault(s => s.State == stateType && s.Event == eventType);
            if (steps == null)
            {
                steps = new SagaAction<TSagaState>()
                {
                    Event = eventType,
                    State = stateType

                };
                this.Add(steps);
            }
            return steps;
        }

        public SagaActions()
        {

        }
    }
    public class SagaAction<TSagaState> where TSagaState : ISagaState
    {
        public Type State;

        public Type Event;

        public List<SagaStep<TSagaState>> Steps =
            new List<SagaStep<TSagaState>>();
    }

    public class SagaStep<TSagaState> where TSagaState : ISagaState
    {
        public ThenFunction<TSagaState> Action;

        public Type Activity;
    }
}