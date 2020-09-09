using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.Steps.Delegates;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaStepForInlineAction<TSagaData> : ISagaStep
        where TSagaData : ISagaData
    {
        public ISagaStep ParentStep { get; }
        public SagaSteps ChildSteps { get; }
        private readonly ThenActionDelegate<TSagaData> compensation;

        public SagaStepForInlineAction(
            string stepName, ThenActionDelegate<TSagaData> action, ThenActionDelegate<TSagaData> compensation,
            bool async, ISagaStep parentStep)
        {
            StepName = stepName;
            this.action = action;
            this.compensation = compensation;
            Async = async;
            ChildSteps = new SagaSteps();
            ParentStep = parentStep;
        }

        private ThenActionDelegate<TSagaData> action { get; }

        public bool Async { get; }
        public string StepName { get; }

        public async Task Compensate(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>) context;

            if (compensation != null)
                await compensation(contextForAction);
        }

        public async Task Execute(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>) context;

            if (action != null)
                await action(contextForAction);
        }
    }
}