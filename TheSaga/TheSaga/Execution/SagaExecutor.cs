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
            this.internalMessageBus.Subscribe<SagaAsyncStepCompletedMessage>(this, HandleAsyncStepCompletedMessage);
        }

        public async Task<ISagaState> Handle(Guid correlationID, IEvent @event)
        {
            Type eventType = @event == null ?
                null : @event.GetType();

            if (eventType != null)
            {
                bool isStartEvent = model.IsStartEvent(eventType);
                if (isStartEvent)
                    correlationID = await createNewSaga(correlationID);
            }

            StepExecutionResult stepExecutionResult = await ExecuteStep(correlationID, @event);
            if (stepExecutionResult.Async || stepExecutionResult.State?.CurrentStep == null)
                return stepExecutionResult.State;

            return await Handle(correlationID, null);
        }

        private Task HandleAsyncStepCompletedMessage(SagaAsyncStepCompletedMessage message)
        {
            if (message.SagaStateType != typeof(TSagaState))
                return Task.CompletedTask;

            Task.Run(async () =>
            {
                try
                {
                    await Handle(message.CorrelationID, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

            return Task.CompletedTask;
        }

        private async Task<StepExecutionResult> ExecuteStep(
            Guid correlationID,
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

            ISagaAction action = null;

            if (eventType != null)
            {
                action = actions.
                    FirstOrDefault(a => a.Event == eventType);

                if (action == null)
                    throw new SagaInvalidEventForStateException(state.CurrentState, eventType);

                if (state.CurrentStep != null)
                    throw new SagaIsBusyHandlingStepException(state.CorrelationID, state.CurrentState, state.CurrentStep);

                state.CurrentStep = action.Steps.First().StepName;

                await sagaPersistance.
                    Set(state);
            }
            else
            {
                action = actions.
                    FirstOrDefault(a => a.FindStep(state.CurrentStep) != null);

                if (action == null)
                    throw new SagaStepNotRegisteredException(state.CurrentState, state.CurrentStep);
            }

            /*
            ISagaStep step = action.
                FindStep(state.CurrentStep);

            if (step == null)
                throw new SagaInvalidEventForStateException(state.CurrentState, eventType);
            */

            bool async = await new SagaStepExecutor<TSagaState>(internalMessageBus, sagaPersistance, @event, state, action).
                Run();

            return new StepExecutionResult()
            {
                State = state,
                Async = async
            };
        }
        private async Task<Guid> createNewSaga(Guid correlationID)
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

        private class StepExecutionResult
        {
            internal bool Async;

            internal ISagaState State;
        }
    }
}