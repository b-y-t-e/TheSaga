using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Interfaces;

namespace TheSaga.States.Actions
{
    public class SagaStepActivity<TSagaState, TSagaActivity> : ISagaStep
        where TSagaState : ISagaState
        where TSagaActivity : ISagaActivity<TSagaState>
    {
        public String StepName { get; private set; }

        public SagaStepActivity(String StepName)
        {
            this.StepName = StepName;
        }

        public async Task Execute(IInstanceContext context, IEvent @event)
        {
            IInstanceContext<TSagaState> contextForAction =
                (IInstanceContext<TSagaState>)context;

            TSagaActivity activity = (TSagaActivity)Activator.CreateInstance(typeof(TSagaActivity));
            if (activity != null)            
                await activity.Execute(contextForAction);            
        }

        public Task Execute(IEventContext context, IEvent @event)
        {
            return Task.CompletedTask;
        }
    }

}