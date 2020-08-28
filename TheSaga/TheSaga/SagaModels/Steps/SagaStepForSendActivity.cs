using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;
using TheSaga.SagaModels.Steps.Delegates;
using TheSaga.SagaModels.Steps.SendMessage;

namespace TheSaga.SagaModels.Steps
{
    internal class SagaStepForSendActivity<TSagaData, TExecuteEvent, TCompensateEvent> : ISagaStep
        where TSagaData : ISagaData
        where TExecuteEvent : IEvent, new()
        where TCompensateEvent : IEvent, new()
    {
        private readonly IServiceProvider serviceProvider;

        private readonly SendActionDelegate<TSagaData, TExecuteEvent> action;

        private readonly SendActionDelegate<TSagaData, TCompensateEvent> compensate;

        public SagaStepForSendActivity(
            SendActionDelegate<TSagaData, TExecuteEvent> action,
            SendActionDelegate<TSagaData, TCompensateEvent> compensate,
            string StepName, IServiceProvider serviceProvider, bool async)
        {
            this.StepName = StepName;
            this.serviceProvider = serviceProvider;
            Async = async;
            this.action = action;
            this.compensate = compensate;
        }

        public bool Async { get; }
        public string StepName { get; }

        public async Task Compensate(IExecutionContext context, IEvent @event)
        {
            if (typeof(TCompensateEvent) == typeof(EmptyEvent))
                return;

            var contextForAction =
                (IExecutionContext<TSagaData>) context;

            var activity =
                ActivatorUtilities.CreateInstance<SendMessageCompensate<TSagaData, TCompensateEvent>>(serviceProvider);

            var eventToSend = new TCompensateEvent();
            if (action != null)
                await compensate(contextForAction, eventToSend);

            if (activity != null)
            {
                await activity.Compensate(
                    contextForAction,
                    eventToSend);
            }
        }

        public async Task Execute(IExecutionContext context, IEvent @event)
        {
            if (typeof(TExecuteEvent) == typeof(EmptyEvent))
                return;

            var contextForAction =
                (IExecutionContext<TSagaData>) context;

            var activity =
                ActivatorUtilities.CreateInstance<SendMessageExecute<TSagaData, TExecuteEvent>>(serviceProvider);

            var eventToSend = new TExecuteEvent();
            if (action != null)
                await action(contextForAction, eventToSend);

            if (activity != null)
            {
                await activity.Execute(
                    contextForAction,
                    eventToSend);
            }
        }
    }
}