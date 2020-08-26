using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.SagaStates.Steps;
using TheSaga.States;

namespace TheSaga.SagaStates.Actions
{
    public class SagaActions<TSagaState>
        where TSagaState : ISagaState
    {
        private List<SagaAction<TSagaState>> actions;

        public SagaActions()
        {
            States = new List<String>();
            DuringEvents = new List<Type>();
            StartEvents = new List<Type>();
            actions = new List<SagaAction<TSagaState>>();
        }

        public List<Type> DuringEvents { get; private set; }
        public List<Type> StartEvents { get; private set; }
        public List<String> States { get; private set; }

        internal void Add(SagaAction<TSagaState> action)
        {
            actions.Add(action);
            Rebuild();
        }

        internal SagaAction<TSagaState> FindAction(string state, Type eventType)
        {
            return this.actions.
                FirstOrDefault(s => s.State == state && s.Event == eventType);
        }

        internal SagaAction<TSagaState> FindAction(ISagaStep sagaStep)
        {
            return this.actions.
                FirstOrDefault(s => s.Steps.Contains(sagaStep));
        }

        internal IList<SagaAction<TSagaState>> FindActions(string state)
        {
            return this.actions.
                Where(s => s.State == state).
                ToArray();
        }

        private void Rebuild()
        {
            States = new List<String>();
            DuringEvents = new List<Type>();
            StartEvents = new List<Type>();

            foreach (var action in actions)
            {
                if (action.Event != null)
                {
                    if (action.State == new SagaStartState().GetStateName())
                    {
                        StartEvents.Add(action.Event);
                    }
                    else
                    {
                        DuringEvents.Add(action.Event);
                    }
                }

                if (action.State != new SagaStartState().GetStateName())
                {
                    States.Add(action.State);
                }
            }
        }
    }
}