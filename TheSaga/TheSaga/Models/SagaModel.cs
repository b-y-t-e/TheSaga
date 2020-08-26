﻿using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;

namespace TheSaga.Models
{
    internal class SagaModel<TSagaState> : ISagaModel<TSagaState>
        where TSagaState : ISagaState
    {
        public SagaModel()
        {
            Actions = new SagaActions<TSagaState>();
            SagaStateType = typeof(TSagaState);
        }

        public SagaActions<TSagaState> Actions { get; }
        public Type SagaStateType { get; }

        public bool ContainsEvent(Type type)
        {
            return
                Actions.StartEvents.Contains(type) ||
                Actions.DuringEvents.Contains(type);
        }

        public ISagaAction FindActionForStep(ISagaStep sagaStep)
        {
            return this.Actions.
                FindAction(sagaStep);
        }

        public ISagaAction FindActionOrCreateForStateAndEvent(string state, Type eventType)
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

        public IList<ISagaAction> FindActionsForState(string state)
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
    }
}