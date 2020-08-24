using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Builders;

namespace TheSaga.States.Actions
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
}