using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.History;
using TheSaga.ModelsSaga.Steps.Delegates;
using TheSaga.ModelsSaga.Steps.Interfaces;

namespace TheSaga.ModelsSaga.Steps
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

        public async Task Compensate(
            IServiceProvider serviceProvider,
            IExecutionContext context, 
            ISagaEvent @event, 
            IStepData stepData)
        {
            if (typeof(TCompensateEvent) == typeof(EmptyEvent))
                return;

            ISagaCoordinator sagaCoordinator = serviceProvider.
                GetRequiredService<ISagaCoordinator>();

            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            TCompensateEvent compensationEvent = new TCompensateEvent();
            if (compensate != null)
                await compensate(contextForAction, compensationEvent);

            await sagaCoordinator.
                Publish(compensationEvent, contextForAction.ExecutionValues);
        }

        public async Task Execute(
            IServiceProvider serviceProvider,
            IExecutionContext context, 
            ISagaEvent @event, 
            IStepData stepData)
        {
            if (typeof(TExecuteEvent) == typeof(EmptyEvent))
                return;

            ISagaCoordinator sagaCoordinator = serviceProvider.
                GetRequiredService<ISagaCoordinator>();

            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            TExecuteEvent executionEvent = new TExecuteEvent();
            if (action != null)
                await action(contextForAction, executionEvent);

            await sagaCoordinator.
                Publish(executionEvent, contextForAction.ExecutionValues);
        }
    }
}
