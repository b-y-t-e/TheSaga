using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models.Interfaces;
using TheSaga.Models.History;
using TheSaga.ModelsSaga.Steps.Delegates;
using TheSaga.ModelsSaga.Steps.Interfaces;
using TheSaga.Options;

namespace TheSaga.ModelsSaga.Steps
{
    public class SagaStepForPublishActivity<TSagaData, TExecuteEvent, TCompensateEvent> : ISagaStep, ISagaPublishActivity<TSagaData, TExecuteEvent, TCompensateEvent>
        where TSagaData : ISagaData
        where TExecuteEvent : ISagaEvent, new()
        where TCompensateEvent : ISagaEvent, new()
    {
        public SendActionAsyncDelegate<TSagaData, TExecuteEvent> ActionDelegate { get; set; }
        public SendActionAsyncDelegate<TSagaData, TCompensateEvent> CompensateDelegate { get; set; }
        public SagaSteps ChildSteps { get; private set; }
        public ISagaStep ParentStep { get; set; }
        public bool Async { get; set; }
        public string StepName { get; set; }

        public SagaStepForPublishActivity(
             /* SendActionAsyncDelegate<TSagaData, TExecuteEvent> action,
              SendActionAsyncDelegate<TSagaData, TCompensateEvent> compensate,
              string StepName, bool async, ISagaStep parentStep*/)
        {
            //this.StepName = StepName;
            //Async = async;
            //this.action = action;
            //this.compensate = compensate;
            ChildSteps = new SagaSteps();
            //ParentStep = parentStep;
        }

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
            if (CompensateDelegate != null)
                await CompensateDelegate(contextForAction, compensationEvent);

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

            ISagaInternalCoordinator sagaCoordinator = serviceProvider.
                GetRequiredService<ISagaCoordinator>() as ISagaInternalCoordinator;

            IExecutionContext<TSagaData> contextForAction =
                (IExecutionContext<TSagaData>)context;

            TExecuteEvent executionEvent = new TExecuteEvent();
            if (ActionDelegate != null)
                await ActionDelegate(contextForAction, executionEvent);

            ISaga newSaga = await sagaCoordinator.
                Publish(executionEvent, contextForAction.ExecutionValues, contextForAction.Data.ID, new SagaRunOptions());
        }
    }
}
