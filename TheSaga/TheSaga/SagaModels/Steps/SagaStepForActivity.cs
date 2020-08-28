using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Activities;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaStepForActivity<TSagaData, TSagaActivity> : ISagaStep
        where TSagaData : ISagaData
        where TSagaActivity : ISagaActivity<TSagaData>
    {
        private readonly IServiceProvider serviceProvider;

        public SagaStepForActivity(
            string StepName, IServiceProvider serviceProvider, bool async)
        {
            this.StepName = StepName;
            this.serviceProvider = serviceProvider;
            Async = async;
        }

        public bool Async { get; }
        public string StepName { get; }

        public async Task Compensate(IExecutionContext context, IEvent @event)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>) context;

            TSagaActivity activity = (TSagaActivity) ActivatorUtilities.CreateInstance(serviceProvider, typeof(TSagaActivity));

            if (activity != null)
                await activity.Compensate(contextForAction);
        }

        public async Task Execute(IExecutionContext context, IEvent @event)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>) context;

            TSagaActivity activity = (TSagaActivity) ActivatorUtilities.CreateInstance(serviceProvider, typeof(TSagaActivity));

            if (activity != null)
                await activity.Execute(contextForAction);
        }
    }
}