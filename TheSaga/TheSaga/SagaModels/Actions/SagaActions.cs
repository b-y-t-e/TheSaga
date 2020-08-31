using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Models;
using TheSaga.SagaModels.Steps;
using TheSaga.States;

namespace TheSaga.SagaModels.Actions
{
    public class SagaActions<TSagaData>
        where TSagaData : ISagaData
    {
        private readonly List<SagaAction<TSagaData>> actions;

        public SagaActions()
        {
            States = new List<string>();
            DuringEvents = new List<Type>();
            StartEvents = new List<Type>();
            actions = new List<SagaAction<TSagaData>>();
        }

        internal SagaActions(IEnumerable<SagaAction<TSagaData>> actions) :
            this()
        {
            this.actions.AddRange(actions);
            Rebuild();
        }

        public List<Type> DuringEvents { get; private set; }
        public List<Type> StartEvents { get; private set; }
        public List<string> States { get; private set; }

        internal void Add(SagaAction<TSagaData> action)
        {
            actions.Add(action);
            Rebuild();
        }

        internal SagaActions<TSagaData> FindActionsByState(string state)
        {
            return new SagaActions<TSagaData>(
                actions.Where(s => s.State == state));
        }
        internal SagaAction<TSagaData> FindActionByStep(string step)
        {
            return actions.FirstOrDefault(a => a.FindStep(step) != null);
        }
        internal SagaAction<TSagaData> FindActionByStep(ISagaStep sagaStep)
        {
            return actions.FirstOrDefault(s => s.Steps.Contains(sagaStep));
        }
        internal SagaAction<TSagaData> FindActionByEventType(Type eventType)
        {
            return actions.FirstOrDefault(a => a.Event == eventType);
        }
        internal SagaAction<TSagaData> FindActionByStateAndEventType(string state, Type eventType)
        {
            return actions.FirstOrDefault(s => s.State == state && s.Event == eventType);
        }
        private void Rebuild()
        {
            States = new List<string>();
            DuringEvents = new List<Type>();
            StartEvents = new List<Type>();

            foreach (SagaAction<TSagaData> action in actions)
            {
                if (action.Event != null)
                {
                    if (action.State == new SagaStartState().GetStateName())
                        StartEvents.Add(action.Event);
                    else
                        DuringEvents.Add(action.Event);
                }

                if (action.State != new SagaStartState().GetStateName()) States.Add(action.State);
            }
        }
    }
}