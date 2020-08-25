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
        private ThenActionDelegate<TSagaState> compensation;

        private ThenActionDelegate<TSagaState> action { get;  }
        public bool Async { get; }
        public String StepName { get; }
        public SagaStepForInlineAction(
            String stepName, ThenActionDelegate<TSagaState> action, ThenActionDelegate<TSagaState> compensation, Boolean async)
        {
            this.StepName = stepName;
            this.action = action;
            this.compensation = compensation;
            Async = async;
        }


        public async Task Compensate(IExecutionContext context, IEvent @event)
        {
            IExecutionContext<TSagaState> contextForAction =
                (IExecutionContext<TSagaState>)context;

            if (compensation != null)
                await compensation(contextForAction);
        }

        public async Task Execute(IExecutionContext context, IEvent @event)
        {
            IExecutionContext<TSagaState> contextForAction =
                (IExecutionContext<TSagaState>)context;

            if (action != null)
                await action(contextForAction);
        }
    }
}