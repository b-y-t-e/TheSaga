using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.History;
using TheSaga.SagaModels.Steps.Delegates;
using TheSaga.SagaModels.Steps.SendMessage;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaStepForSendActivity<TSagaData, TExecuteEvent, TCompensateEvent> : ISagaStep
        where TSagaData : ISagaData
        where TExecuteEvent : ISagaEvent, new()
        where TCompensateEvent : ISagaEvent, new()
    {
        private readonly SendActionDelegate<TSagaData, TExecuteEvent> action;

        private readonly SendActionDelegate<TSagaData, TCompensateEvent> compensate;
        public SagaSteps ChildSteps { get; }
        public ISagaStep ParentStep { get; }

        public SagaStepForSendActivity(
            SendActionDelegate<TSagaData, TExecuteEvent> action,
            SendActionDelegate<TSagaData, TCompensateEvent> compensate,
            string StepName, bool async, ISagaStep parentStep)
        {
            this.StepName = StepName;
            Async = async;
            this.action = action;
            this.compensate = compensate;
            ChildSteps = new SagaSteps();
            ParentStep = parentStep;
        }

        public bool Async { get; }
        public string StepName { get; }

        public async Task Compensate(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            if (typeof(TCompensateEvent) == typeof(EmptyEvent))
                return;

            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>) context;

            SendMessageCompensate<TSagaData, TCompensateEvent> activity =
                ActivatorUtilities.CreateInstance<SendMessageCompensate<TSagaData, TCompensateEvent>>(serviceProvider);

            TCompensateEvent eventToSend = new TCompensateEvent();
            if (action != null)
                await compensate(contextForAction, eventToSend);

            if (activity != null)
                await activity.Compensate(
                    contextForAction,
                    eventToSend);
        }

        public async Task Execute(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            if (typeof(TExecuteEvent) == typeof(EmptyEvent))
                return;

            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>) context;

            SendMessageExecute<TSagaData, TExecuteEvent> activity =
                ActivatorUtilities.CreateInstance<SendMessageExecute<TSagaData, TExecuteEvent>>(serviceProvider);

            TExecuteEvent eventToSend = new TExecuteEvent();
            if (action != null)
                await action(contextForAction, eventToSend);

            if (activity != null)
                await activity.Execute(
                    contextForAction,
                    eventToSend);
        }
    }
}