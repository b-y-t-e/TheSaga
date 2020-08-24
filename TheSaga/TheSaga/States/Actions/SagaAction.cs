using System;
using System.Collections.Generic;

namespace TheSaga.States.Actions
{
    public class SagaAction<TSagaState> where TSagaState : ISagaState
    {
        public Type State;

        public Type Event;

        public List<SagaStep<TSagaState>> Steps =
            new List<SagaStep<TSagaState>>();
    }
}