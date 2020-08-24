using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheSaga.States;
using TheSaga.States.Actions;

namespace TheSaga.Models
{
    public class SagaModel<TSagaState> : ISagaModel
        where TSagaState : ISagaState
    {
        public Type SagaStateType { get; }
        public List<Type> States { get; private set; }
        public List<Type> StartEvents { get; private set; }
        public List<Type> DuringEvents { get; private set; }
        public SagaActions<TSagaState> Actions { get; }

        public SagaModel()
        {
            States = new List<Type>();
            DuringEvents = new List<Type>();
            StartEvents = new List<Type>();
            Actions = new SagaActions<TSagaState>();
            SagaStateType = typeof(TSagaState);
        }

        internal void Build()
        {

            States = new List<Type>();
            DuringEvents = new List<Type>();

            foreach (var action in Actions)
            {
                if (action.Event != null)
                {
                    if (action.State == null)
                    {
                        StartEvents.Add(action.Event);
                    }
                    else
                    {
                        DuringEvents.Add(action.Event);
                    }
                }

                if (action.State != null)
                {
                    States.Add(action.State);
                }
            }
        }

        public bool IsStartEvent(Type type)
        {
            return StartEvents.Contains(type);
        }

        public bool ContainsEvent(Type type)
        {
            return
                StartEvents.Contains(type) ||
                DuringEvents.Contains(type);
        }
    }
}