using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheSaga.Conditions;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.History;
using TheSaga.SagaModels.Steps.Delegates;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaStepForElseIfInline<TSagaData> : ISagaStep, ISagaStepForIf, ISagaStepElse
        where TSagaData : ISagaData
    {
        public ISagaStep ParentStep { get; }
        public SagaSteps ChildSteps { get; private set; }

        public SagaStepForElseIfInline(
            string StepName,
            IfFuncDelegate<TSagaData> action,
            ThenActionDelegate<TSagaData> compensation,
            ISagaStep parentStep)
        {
            this.StepName = StepName;
            this.action = action;
            this.compensation = compensation;
            this.Async = false;
            this.ChildSteps = new SagaSteps();
            this.ParentStep = parentStep;
        }

        public bool Async { get; }
        public string StepName { get; }
        private IfFuncDelegate<TSagaData> action { get; }
        private ThenActionDelegate<TSagaData> compensation { get; }

        public void SetChildSteps(SagaSteps steps)
        {
            this.ChildSteps = steps;
        }

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
            {
                bool result = await action(contextForAction);
                stepData.ExecutionData.ConditionResult = result;
            }
            else
            {
                stepData.ExecutionData.ConditionResult = false;
            }
        }
    }
}