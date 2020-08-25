using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.States;

namespace TheSaga.Execution
{
    internal class SagaExecutor<TSagaState> : ISagaExecutor
        where TSagaState : ISagaState
    {
        private ISagaPersistance sagaPersistance;

        public SagaExecutor(ISagaPersistance sagaPersistance)
        {
            this.sagaPersistance = sagaPersistance;
        }

        public async Task<ISagaState> Handle(Guid correlationID, ISagaModel model, IEvent @event)
        {
            Type eventType = @event.GetType();

            bool isStartEvent = model.IsStartEvent(eventType);
            if (isStartEvent)
            {
                correlationID = await createNewSaga(correlationID, model);
            }

            while (true)
            {
                StepExecutionResult stepExecutionResult = await ExecuteStep(correlationID, model, @event);
                if (stepExecutionResult.Async || stepExecutionResult.State?.CurrentStep == null)
                    return stepExecutionResult.State;
            }
        }

        private async Task<Guid> createNewSaga(Guid correlationID, ISagaModel model)
        {
            if (correlationID == Guid.Empty)
                correlationID = Guid.NewGuid();

            ISagaState newSagaState = (ISagaState)Activator.CreateInstance(model.SagaStateType);
            newSagaState.CorrelationID = correlationID;
            newSagaState.CurrentState = Extensions.GetStateName<SagaStartState>();
            newSagaState.CurrentStep = null;

            await sagaPersistance.Set(newSagaState);
            return correlationID;
        }

        private async Task<StepExecutionResult> ExecuteStep(
                    Guid correlationID,
            ISagaModel model,
            IEvent @event)
        {
            Type eventType = @event == null ?
                null : @event.GetType();

            ISagaState state = await sagaPersistance.
                Get(correlationID);

            if (state == null)
                throw new SagaInstanceNotFoundException(model.SagaStateType, correlationID);

            IList<ISagaAction> actions = model.
                FindActions(state.CurrentState);

            ISagaAction action = actions.
                FirstOrDefault(a => a.Event == eventType);

            if (action == null)
                throw new SagaInvalidEventForStateException(state.CurrentState, eventType);

            bool async = await new SagaStepExecutor<TSagaState>(sagaPersistance, @event, state, action).
                Run();

            return new StepExecutionResult()
            {
                State = state,
                Async = async
            };
        }

        private class StepExecutionResult
        {
            internal bool Async;

            internal ISagaState State;
        }
    }
}