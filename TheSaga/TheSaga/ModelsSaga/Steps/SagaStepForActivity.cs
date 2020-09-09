using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaStepForActivity<TSagaData, TSagaActivity> : ISagaStep
        where TSagaData : ISagaData
        where TSagaActivity : ISagaActivity<TSagaData>
    {
        private readonly IServiceProvider serviceProvider;

        public ISagaStep ParentStep { get; }
        public SagaSteps ChildSteps { get; }
        public SagaStepForActivity(
            string StepName, IServiceProvider serviceProvider, bool async, ISagaStep parentStep)
        {
            this.StepName = StepName;
            this.serviceProvider = serviceProvider;
            Async = async;
            ChildSteps = new SagaSteps();
            ParentStep = parentStep;
        }

        public bool Async { get; }
        public string StepName { get; }

        public async Task Compensate(IExecutionContext context, ISagaEvent @event)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            TSagaActivity activity = (TSagaActivity)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TSagaActivity));

            if (activity != null)
                await activity.Compensate(contextForAction);
        }

        public async Task Execute(IExecutionContext context, ISagaEvent @event)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            TSagaActivity activity = (TSagaActivity)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TSagaActivity));

            if (activity != null)
                await activity.Execute(contextForAction);
        }
    }
}