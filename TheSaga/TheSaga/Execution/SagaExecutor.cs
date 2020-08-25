using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Messages;
using TheSaga.Messages.MessageBus;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;
using TheSaga.States;

namespace TheSaga.Execution
{
    internal class SagaExecutor<TSagaState> : ISagaExecutor
        where TSagaState : ISagaState
    {
        private ISagaModel<TSagaState> model;
        private ISagaPersistance sagaPersistance;
        private IInternalMessageBus internalMessageBus;

        public SagaExecutor(ISagaModel<TSagaState> model, ISagaPersistance sagaPersistance, IInternalMessageBus internalMessageBus)
        {
            this.model = model;
            this.sagaPersistance = sagaPersistance;
            this.internalMessageBus = internalMessageBus;

            new SagaAsyncStepHandler<TSagaState>(this, sagaPersistance, internalMessageBus).
                Subscribe();
        }

        public async Task<ISagaState> Handle(Guid correlationID, IEvent @event, Boolean @async)
        {
            Type eventType = @event == null ?
                null : @event.GetType();

            if (eventType != null)
            {
                bool isStartEvent = model.IsStartEvent(eventType);
                if (isStartEvent)
                    correlationID = await CreateNewSaga(correlationID);
            }

            SagaStepExecutor<TSagaState> stepExecutor =
                new SagaStepExecutor<TSagaState>(correlationID, async, @event, model, internalMessageBus, sagaPersistance);

            StepExecutionResult stepExecutionResult = await stepExecutor.ExecuteStep();
            if (stepExecutionResult.Async || stepExecutionResult.State?.CurrentStep == null)
                return stepExecutionResult.State;

            return await Handle(correlationID, null, @async);
        }

        private async Task<Guid> CreateNewSaga(Guid correlationID)
        {
            if (correlationID == Guid.Empty)
                correlationID = Guid.NewGuid();

            ISagaState newSagaState = (ISagaState)Activator.CreateInstance(model.SagaStateType);
            newSagaState.CorrelationID = correlationID;
            newSagaState.CurrentState = new SagaStartState().GetStateName();
            newSagaState.CurrentStep = null;

            await sagaPersistance.Set(newSagaState);
            return correlationID;
        }
    }
}