using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.History;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaStepForThen<TSagaData, TSagaActivity> : ISagaStep
        where TSagaData : ISagaData
        where TSagaActivity : ISagaActivity<TSagaData>
    {
        public ISagaStep ParentStep { get; }
        public SagaSteps ChildSteps { get; }
        public SagaStepForThen(
            string StepName, bool async, ISagaStep parentStep)
        {
            this.StepName = StepName;
            Async = async;
            ChildSteps = new SagaSteps();
            ParentStep = parentStep;
        }

        public bool Async { get; }
        public string StepName { get; }

        public async Task Compensate(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            TSagaActivity activity = (TSagaActivity)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TSagaActivity));

            if (activity != null)
                await activity.Compensate(contextForAction);
        }

        public async Task Execute(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            TSagaActivity activity = (TSagaActivity)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TSagaActivity));

            if (activity != null)
                await activity.Execute(contextForAction);
        }
    }
}