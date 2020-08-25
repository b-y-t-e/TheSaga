using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Events;
using TheSaga.Execution.Context;

namespace TheSaga.SagaStates.Steps
{
    public class SagaStepForInlineAction<TSagaState> : ISagaStep
        where TSagaState : ISagaState
    {
        public String StepName { get; private set; }

        public ThenActionDelegate<TSagaState> Action { get; private set; }
        public bool Async { get; }

        public SagaStepForInlineAction(
            String StepName, ThenActionDelegate<TSagaState> Action, Boolean async)
        {
            this.StepName = StepName;
            this.Action = Action;
            Async = async;
        }

        public async Task Run(IExecutionContext context, IEvent @event)
        {
            IExecutionContext<TSagaState> contextForAction =
                (IExecutionContext<TSagaState>)context;

            if (Action != null)            
                await Action(contextForAction);            
        }
    }

    public delegate Task ThenActionDelegate<TSagaState>(IExecutionContext<TSagaState> context)
        where TSagaState : ISagaState;
}