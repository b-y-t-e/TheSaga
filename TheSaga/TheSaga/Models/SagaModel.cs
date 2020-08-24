using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheSaga.States;
using TheSaga.States.Actions;

namespace TheSaga.Models
{
    public class SagaModel<TSagaState> : ISagaModel<TSagaState>
        where TSagaState : ISagaState
    {
        public Type SagaStateType { get; }
        public SagaActions<TSagaState> Actions { get; }
        public SagaModel()
        {
            Actions = new SagaActions<TSagaState>();
            SagaStateType = typeof(TSagaState);
        }

        public ISagaAction FindAction(string state, Type eventType)
        {
            SagaAction<TSagaState> action = this.Actions.
                FindAction(state, eventType);

            if (action == null)
            {
                action = new SagaAction<TSagaState>()
                {
                    Event = eventType,
                    State = state
                };
                this.Actions.Add(action);
            }

            return action;
        }

        public IList<ISagaAction> FindActions(string state)
        {
            return this.Actions.
                 FindActions(state).
                 Select(i => (ISagaAction)i).
                 ToArray();
        }

        public bool IsStartEvent(Type type)
        {
            return Actions.StartEvents.Contains(type);
        }

        public bool ContainsEvent(Type type)
        {
            return
                Actions.StartEvents.Contains(type) ||
                Actions.DuringEvents.Contains(type);
        }
    }
}