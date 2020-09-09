using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheSaga.Conditions;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.History;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaStepIf<TSagaData, TSagaCondition> : ISagaStep
        where TSagaData : ISagaData
        where TSagaCondition : ISagaCondition<TSagaData>
    {
        public ISagaStep ParentStep { get; }
        public SagaSteps ChildSteps { get; private set; }
        public Type PolicyType { get; }

        public SagaStepIf(string StepName, ISagaStep parentStep)
        {
            this.PolicyType = typeof(TSagaCondition);
            this.StepName = StepName;
            this.Async = false;
            this.ChildSteps = new SagaSteps();
            this.ParentStep = parentStep;
        }

        public bool Async { get; }
        public string StepName { get; }

        public void SetChildSteps(SagaSteps steps)
        {
            this.ChildSteps = steps;
        }

        public async Task Compensate(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            TSagaCondition activity = (TSagaCondition)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TSagaCondition));

            if (activity != null)
                await activity.Compensate(contextForAction);
        }

        public async Task Execute(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            TSagaCondition activity = (TSagaCondition)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TSagaCondition));

            if (activity != null)
            {
                bool result = await activity.Execute(contextForAction);
                stepData.ExecutionData.ConditionResult = result;
            }
        }
    }
}