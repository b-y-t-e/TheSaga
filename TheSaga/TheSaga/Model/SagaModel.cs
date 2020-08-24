using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheSaga.Model
{
    public class SagaModel<TSagaState> : ISagaModel
        where TSagaState : ISagaState 
    {
        public List<Type> States { get; private set; }
        public List<Type> Events { get; private set; }
        public SagaActions<TSagaState> Actions { get;  }
        //internal List<SagaSteps> Durings { get; }

        public SagaModel()
        {
            States = new List<Type>();
            Events = new List<Type>();
            //Starts = new List<SagaSteps>();
            Actions = new SagaActions<TSagaState>();
        }



        internal void Build()
        {

            States = new List<Type>();
            Events = new List<Type>();

            foreach( var action in Actions)
            {
                if (action.Event != null)
                    Events.Add(action.Event);

                if (action.State != null)
                    States.Add(action.State);
            }
        }



    }

    public interface ISagaModel
    {
        bool IsStartEvent(Type type);
    }
}