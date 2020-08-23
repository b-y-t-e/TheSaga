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

        public SagaSteps<TSagaState> GetDuring(Type stateType, Type eventType)
        {
            var steps = this.FirstOrDefault(s => s.State == stateType && s.Event == eventType);
            if (steps == null)
            {
                steps = new SagaSteps<TSagaState>()
                {
                    Event = eventType,
                    State = stateType

                };
                this.Add(steps);
            }
            return steps;
        }

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

    public class SagaStep<TSagaState> where TSagaState : ISagaState
    {
        public ThenFunction<TSagaState> Action;

        public Type Activity;
    }
}