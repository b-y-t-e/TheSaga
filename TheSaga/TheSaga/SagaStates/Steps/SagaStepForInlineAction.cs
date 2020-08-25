using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Execution.Context;

namespace TheSaga.SagaStates.Steps
{
    public delegate Task ThenActionDelegate<TSagaState>(IExecutionContext<TSagaState> context)
        where TSagaState : ISagaState;

    internal class SagaStepForInlineAction<TSagaState> : ISagaStep
            where TSagaState : ISagaState
    {
        public SagaStepForInlineAction(
            String StepName, ThenActionDelegate<TSagaState> Action, Boolean async)
        {
            this.StepName = StepName;
            this.Action = Action;
            Async = async;
        }

        public ThenActionDelegate<TSagaState> Action { get; private set; }
        public bool Async { get; }
        public String StepName { get; private set; }

        public Task Compensate(IExecutionContext context, IEvent @event)
        {
            return Task.CompletedTask;
        }

        public async Task Execute(IExecutionContext context, IEvent @event)
        {
            IExecutionContext<TSagaState> contextForAction =
                (IExecutionContext<TSagaState>)context;

            if (Action != null)
                await Action(contextForAction);
        }
    }
}