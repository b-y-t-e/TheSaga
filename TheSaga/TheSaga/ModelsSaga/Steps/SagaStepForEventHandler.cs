using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.History;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaStepForEventHandler<TSagaData, TEventHandler, TEvent> : ISagaStep
        where TSagaData : ISagaData
        where TEventHandler : ISagaEventHandler<TSagaData, TEvent>
        where TEvent : ISagaEvent
    {
        public ISagaStep ParentStep { get; }
        public SagaSteps ChildSteps { get; }

        public SagaStepForEventHandler(
            string StepName, bool async, ISagaStep parentStep)
        {
            this.StepName = StepName;
            Async = async;
            ChildSteps = new SagaSteps();
            ParentStep = parentStep;
        }

        public bool Async { get; }
        public string StepName { get; }

        public async Task Compensate(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>) context;

            TEventHandler activity = (TEventHandler) ActivatorUtilities.CreateInstance(serviceProvider, typeof(TEventHandler));

            if (activity != null)
                await activity.Compensate(contextForAction, (TEvent) @event);
        }

        public async Task Execute(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>) context;

            TEventHandler activity = (TEventHandler) ActivatorUtilities.CreateInstance(serviceProvider, typeof(TEventHandler));

            if (activity != null)
                await activity.Execute(contextForAction, (TEvent) @event);
        }
    }
}