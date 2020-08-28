using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;
using TheSaga.SagaModels.Steps.Delegates;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaStepForInlineAction<TSagaData> : ISagaStep
            where TSagaData : ISagaData
    {
        private ThenActionDelegate<TSagaData> compensation;

        public SagaStepForInlineAction(
            String stepName, ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation, Boolean async)
        {
            this.StepName = stepName;
            this.action = action;
            this.compensation = compensation;
            Async = async;
        }

        public bool Async { get; }
        public String StepName { get; }
        private ThenActionDelegate<TSagaData> action { get; }

        public async Task Compensate(IExecutionContext context, IEvent @event)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            if (compensation != null)
                await compensation(contextForAction);
        }

        public async Task Execute(IExecutionContext context, IEvent @event)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            if (action != null)
                await action(contextForAction);
        }
    }
}