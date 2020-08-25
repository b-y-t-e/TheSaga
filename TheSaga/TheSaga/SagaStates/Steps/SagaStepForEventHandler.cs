using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Execution.Context;

namespace TheSaga.SagaStates.Steps
{
    internal class SagaStepForEventHandler<TSagaState, TEventHandler, TEvent> : ISagaStep
        where TSagaState : ISagaState
        where TEventHandler : IEventHandler<TSagaState, TEvent>
        where TEvent : IEvent
    {
        private readonly IServiceProvider serviceProvider;

        public SagaStepForEventHandler(
            String StepName, IServiceProvider serviceProvider, Boolean async)
        {
            this.StepName = StepName;
            this.serviceProvider = serviceProvider;
            Async = async;
        }

        public bool Async { get; }
        public String StepName { get; private set; }

        public async Task Compensate(IExecutionContext context, IEvent @event)
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
                await activity.Compensate(eventContext);
        }

        public async Task Execute(IExecutionContext context, IEvent @event)
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