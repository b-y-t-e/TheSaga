using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Interfaces;

namespace TheSaga.States.Actions
{
    public class SagaStepForInlineAction<TSagaState> : ISagaStep
        where TSagaState : ISagaState
    {
        public String StepName { get; private set; }

        public ThenActionDelegate<TSagaState> Action { get; private set; }

        public SagaStepForInlineAction(String StepName, ThenActionDelegate<TSagaState> Action)
        {
            this.StepName = StepName;
            this.Action = Action;
        }

        public async Task Execute(IInstanceContext context, IEvent @event)
        {
            IInstanceContext<TSagaState> contextForAction =
                (IInstanceContext<TSagaState>)context;

            if (Action != null)            
                await Action(contextForAction);            
        }
    }

    public delegate Task ThenActionDelegate<TSagaState>(IInstanceContext<TSagaState> context)
        where TSagaState : ISagaState;
}