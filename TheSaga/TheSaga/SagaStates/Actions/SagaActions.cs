using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Builders;
using TheSaga.States;

namespace TheSaga.SagaStates.Actions
{
    public class SagaActions<TSagaState>
        where TSagaState : ISagaState
    {
        private List<SagaAction<TSagaState>> items;

        public List<String> States { get; private set; }
        public List<Type> StartEvents { get; private set; }
        public List<Type> DuringEvents { get; private set; }

        public SagaActions()
        {
            States = new List<String>();
            DuringEvents = new List<Type>();
            StartEvents = new List<Type>();
            items = new List<SagaAction<TSagaState>>();
        }

        void Rebuild()
        {
            States = new List<String>();
            DuringEvents = new List<Type>();
            StartEvents = new List<Type>();

            foreach (var action in items)
            {
                if (action.Event != null)
                {
                    if (action.State == SagaStartState.Name)
                    {
                        StartEvents.Add(action.Event);
                    }
                    else
                    {
                        DuringEvents.Add(action.Event);
                    }
                }

                if (action.State != SagaStartState.Name)
                {
                    States.Add(action.State);
                }
            }
        }

        internal SagaAction<TSagaState> FindAction(string state, Type eventType)
        {
            return this.items.
                FirstOrDefault(s => s.State == state && s.Event == eventType);
        }

        internal IList<SagaAction<TSagaState>> FindActions(string state)
        {
            return this.items.
                Where(s => s.State == state).
                ToArray();
        }

        internal void Add(SagaAction<TSagaState> action)
        {
            items.Add(action);
            Rebuild();
        }
    }
}