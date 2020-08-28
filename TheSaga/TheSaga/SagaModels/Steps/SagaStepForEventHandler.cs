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
            var contextForAction =
                (IExecutionContext<TSagaData>) context;

            var activity = (TEventHandler) ActivatorUtilities.CreateInstance(serviceProvider, typeof(TEventHandler));

            if (activity != null)
                await activity.Compensate(contextForAction, (TEvent) @event);
        }

        public async Task Execute(IExecutionContext context, IEvent @event)
        {
            var contextForAction =
                (IExecutionContext<TSagaData>) context;

            var activity = (TEventHandler) ActivatorUtilities.CreateInstance(serviceProvider, typeof(TEventHandler));

            if (activity != null)
                await activity.Execute(contextForAction, (TEvent) @event);
        }
    }
}