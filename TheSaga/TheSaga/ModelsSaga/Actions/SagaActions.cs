using System;
using System.Collections;
using System.Collections.Generic;
using TheSaga.ModelsSaga.Actions.Interfaces;
using TheSaga.States;

namespace TheSaga.ModelsSaga.Actions
{
    internal class SagaActions : ISagaActions
    {
        private List<ISagaAction> actions;

        private List<Type> duringEvents;

        private List<Type> startEvents;

        private List<String> states;

        public SagaActions()
        {
            actions = new List<ISagaAction>();
            states = new List<String>();
            duringEvents = new List<Type>();
            startEvents = new List<Type>();
        }

        public SagaActions(IEnumerable<ISagaAction> actions) : this()
        {
            this.actions.AddRange(actions);
            Rebuild();
        }

        public void Add(ISagaAction action)
        {
            actions.Add(action);
            Rebuild();
        }

        public bool IsEventSupported(
             Type type)

        {
            return
                startEvents.Contains(type) ||
                duringEvents.Contains(type);
        }

        public IEnumerator<ISagaAction> GetEnumerator()
        {
            return this.actions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.actions.GetEnumerator();
        }

        public bool IsStartEvent(
             Type type)

        {
            return startEvents.Contains(type);
        }

        private void Rebuild()
        {
            states = new List<string>();
            duringEvents = new List<Type>();
            startEvents = new List<Type>();

            foreach (SagaAction action in actions)
            {
                if (action.Event != null)
                {
                    if (action.State == new SagaStartState().GetStateName())
                        startEvents.Add(action.Event);
                    else
                        duringEvents.Add(action.Event);
                }

                if (action.State != new SagaStartState().GetStateName())
                    states.Add(action.State);
            }
        }
    }
}
