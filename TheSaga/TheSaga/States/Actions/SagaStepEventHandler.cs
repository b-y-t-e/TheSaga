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

        public async Task Execute(IInstanceContext context, IEvent @event)
        {
            IInstanceContext<TSagaState> contextForAction =
                (IInstanceContext<TSagaState>)context;

            var eventContext = new EventContext<TSagaState, TEvent>()
            {
                Event = (TEvent)@event,
                State = contextForAction.State
            };

            // ActivatorUtilities.CreateInstance

            TEventHandler activity = (TEventHandler)Activator.CreateInstance(typeof(TEventHandler));
            if (activity != null)
                await activity.Execute(eventContext);
        }

        public Task Execute(IEventContext context, IEvent @event)
        {
            return Task.CompletedTask;
        }
    }

}