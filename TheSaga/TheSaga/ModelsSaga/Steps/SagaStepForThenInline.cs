using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.History;
using TheSaga.ModelsSaga.Steps.Delegates;
using TheSaga.ModelsSaga.Steps.Interfaces;

namespace TheSaga.ModelsSaga.Steps
{
    internal class SagaStepForThenInline<TSagaData> : ISagaStep
        where TSagaData : ISagaData
    {
        public SagaStepForThenInline(
            string stepName,
            ThenActionDelegate<TSagaData> action,
            ThenActionDelegate<TSagaData> compensation,
            bool async, ISagaStep parentStep)
        {
            StepName = stepName;
            this.action = action;
            this.compensation = compensation;
            Async = async;
            ChildSteps = new SagaSteps();
            ParentStep = parentStep;
        }

        public bool Async { get; }
        public SagaSteps ChildSteps { get; }
        public ISagaStep ParentStep { get; }
        public string StepName { get; }
        private ThenActionDelegate<TSagaData> action { get; }
        private ThenActionDelegate<TSagaData> compensation { get; }

        public async Task Compensate(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            if (compensation != null)
                await compensation(contextForAction);
        }

        public async Task Execute(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            if (action != null)
                await action(contextForAction);
        }
    }
}
