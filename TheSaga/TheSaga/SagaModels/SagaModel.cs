using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Models;
using TheSaga.Models.Actions;
using TheSaga.Models.Steps;

namespace TheSaga.SagaModels
{
    internal class SagaModel<TSagaData> : ISagaModel<TSagaData>
        where TSagaData : ISagaData
    {
        public SagaModel()
        {
            Name = $"{typeof(TSagaData).Name}Model";
            Actions = new SagaActions<TSagaData>();
            SagaStateType = typeof(TSagaData);
        }

        public SagaActions<TSagaData> Actions { get; }
        public string Name { get; set; }
        public Type SagaStateType { get; }

        public bool ContainsEvent(Type type)
        {
            return
                Actions.StartEvents.Contains(type) ||
                Actions.DuringEvents.Contains(type);
        }

        public ISagaAction FindActionForStateAndEvent(string state, Type eventType)
        {
            SagaAction<TSagaData> action = this.Actions.
                FindAction(state, eventType);

            if (action == null)
                throw new Exception($"Could not find action for state {state} and event of type {eventType?.Name}");

            return action;
        }

        public ISagaAction FindActionForStep(ISagaStep sagaStep)
        {
            return this.Actions.
                FindAction(sagaStep);
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