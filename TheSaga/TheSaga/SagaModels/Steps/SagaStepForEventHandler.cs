using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaStepForEventHandler<TSagaData, TEventHandler, TEvent> : ISagaStep
        where TSagaData : ISagaData
        where TEventHandler : IEventHandler<TSagaData, TEvent>
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
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            TEventHandler activity = (TEventHandler)ActivatorUtilities.
                CreateInstance(serviceProvider, typeof(TEventHandler));

            if (activity != null)
                await activity.Compensate(contextForAction, (TEvent)@event);
        }

        public async Task Execute(IExecutionContext context, IEvent @event)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            TEventHandler activity = (TEventHandler)ActivatorUtilities.
                CreateInstance(serviceProvider, typeof(TEventHandler));

            if (activity != null)
                await activity.Execute(contextForAction, (TEvent)@event);
        }
    }

}