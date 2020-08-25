using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Events;
using TheSaga.Execution.Context;

namespace TheSaga.SagaStates.Steps
{
    public class SagaStepForEventHandler<TSagaState, TEventHandler, TEvent> : ISagaStep
        where TSagaState : ISagaState
        where TEventHandler : IEventHandler<TSagaState, TEvent>
        where TEvent : IEvent
    {
        private readonly IServiceProvider serviceProvider;

        public String StepName { get; private set; }
        public bool Async { get; }

        public SagaStepForEventHandler(
            String StepName, IServiceProvider serviceProvider, Boolean async)
        {
            this.StepName = StepName;
            this.serviceProvider = serviceProvider;
            Async = async;
        }

        public async Task Run(IExecutionContext context, IEvent @event)
        {
            IExecutionContext<TSagaState> contextForAction =
                (IExecutionContext<TSagaState>)context;

            var eventContext = new EventContext<TSagaState, TEvent>()
            {
                Event = (TEvent)@event,
                State = contextForAction.State
            };

            TEventHandler activity = (TEventHandler)ActivatorUtilities.
                CreateInstance(serviceProvider, typeof(TEventHandler));

            if (activity != null)
                await activity.Execute(eventContext);
        }
    }

}