using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Interfaces;

namespace TheSaga.States.Actions
{
    public class SagaStepEventHandler<TSagaState, TEventHandler, TEvent> : ISagaStep
        where TSagaState : ISagaState
        where TEventHandler : IEventHandler<TSagaState, TEvent>
        where TEvent : IEvent
    {
        public String StepName { get; private set; }

        public SagaStepEventHandler(String StepName)
        {
            this.StepName = StepName;
        }

        public Task Execute(IInstanceContext context, IEvent @event)
        {
            return Task.CompletedTask;
        }

        public async Task Execute(IEventContext context, IEvent @event)
        {
            IEventContext<TSagaState, TEvent> contextForAction =
                (IEventContext<TSagaState, TEvent>)context;

            TEventHandler activity = (TEventHandler)Activator.CreateInstance(typeof(TEventHandler));
            if (activity != null)
                await activity.Execute(contextForAction);
        }
    }

}