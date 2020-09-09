using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaStepForEventHandler<TSagaData, TEventHandler, TEvent> : ISagaStep
        where TSagaData : ISagaData
        where TEventHandler : ISagaEventHandler<TSagaData, TEvent>
        where TEvent : ISagaEvent
    {
        private readonly IServiceProvider serviceProvider;
        public ISagaStep ParentStep { get; }
        public SagaSteps ChildSteps { get; }

        public SagaStepForEventHandler(
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
                (IExecutionContext<TSagaData>) context;

            TEventHandler activity = (TEventHandler) ActivatorUtilities.CreateInstance(serviceProvider, typeof(TEventHandler));

            if (activity != null)
                await activity.Compensate(contextForAction, (TEvent) @event);
        }

        public async Task Execute(IExecutionContext context, ISagaEvent @event)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>) context;

            TEventHandler activity = (TEventHandler) ActivatorUtilities.CreateInstance(serviceProvider, typeof(TEventHandler));

            if (activity != null)
                await activity.Execute(contextForAction, (TEvent) @event);
        }
    }
}