﻿using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.History;
using TheSaga.ModelsSaga.Steps.Delegates;
using TheSaga.ModelsSaga.Steps.Interfaces;

namespace TheSaga.ModelsSaga.Steps
{
    internal class SagaStepForElseIfInline<TSagaData> : ISagaStep, ISagaStepForIf, ISagaStepForElse
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
