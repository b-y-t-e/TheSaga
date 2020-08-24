using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Interfaces;

namespace TheSaga.States.Actions
{
    public class SagaStepForActivity<TSagaState, TSagaActivity> : ISagaStep
        where TSagaState : ISagaState
        where TSagaActivity : ISagaActivity<TSagaState>
    {
        private readonly IServiceProvider serviceProvider;

        public String StepName { get; private set; }
        public bool Async { get; }

        public SagaStepForActivity(
            String StepName, IServiceProvider serviceProvider, Boolean async)
        {
            this.StepName = StepName;
            this.serviceProvider = serviceProvider;
            Async = async;
        }

        public async Task Execute(IInstanceContext context, IEvent @event)
        {
            IInstanceContext<TSagaState> contextForAction =
                (IInstanceContext<TSagaState>)context;

            TSagaActivity activity = (TSagaActivity)ActivatorUtilities.
                CreateInstance(serviceProvider, typeof(TSagaActivity));

            if (activity != null)            
                await activity.Execute(contextForAction);            
        }
    }

}