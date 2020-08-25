using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Execution.Context;
using TheSaga.Messages;
using TheSaga.Messages.MessageBus;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;

namespace TheSaga.Execution
{
    internal class SagaStepExecutor<TSagaState>
        where TSagaState : ISagaState
    {
        private IEvent @event;
        private ISagaAction action;
        private Guid correlationID;
        private IInternalMessageBus internalMessageBus;
        private ISagaModel<TSagaState> model;
        private ISagaPersistance sagaPersistance;
        private ISagaState state;

        public SagaStepExecutor(
            Guid correlationID,
            IEvent @event,
            ISagaModel<TSagaState> model,
            IInternalMessageBus internalMessageBus,
            ISagaPersistance sagaPersistance)
        {
            this.correlationID = correlationID;
            this.@event = @event;
            this.model = model;
            this.internalMessageBus = internalMessageBus;
            this.sagaPersistance = sagaPersistance;
        }

        internal async Task<StepExecutionResult> ExecuteStep()
        {
            Type eventType = @event == null ?
                null : @event.GetType();

            this.state = await sagaPersistance.
                Get(correlationID);

            if (state == null)
                throw new SagaInstanceNotFoundException(model.SagaStateType, correlationID);

            IList<ISagaAction> actions = model.
                FindActions(state.CurrentState);

            this.action = null;

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

            ISagaStep step = action.
                FindStep(state.CurrentStep);

            if (step == null)
                throw new SagaStepNotRegisteredException(state.CurrentState, state.CurrentStep);

            bool async = await runStep(step);

            return new StepExecutionResult()
            {
                State = state,
                Async = async
            };
        }

        private async Task<bool> runStep(ISagaStep step)
        {
            if (step.Async)
            {
                RunStepAsync();
                return true;
            }
            else
            {
                await RunStepSync();
                return false;
            }
        }

        private void RunStepAsync()
        {
            Task.Run(() => RunStepSync());
        }

        private async Task RunStepSync()
        {
            try
            {
                string prevState = state.CurrentState;
                string prevStep = state.CurrentStep;

                ISagaStep sagaStep = action.
                    FindStep(state.CurrentStep);

                ISagaStep nextSagaStep = action.
                    FindNextAfter(sagaStep);

                IExecutionContext context = new ExecutionContext<TSagaState>()
                {
                    State = (TSagaState)state
                };

                await sagaStep.
                    Run(context, @event);

                if (nextSagaStep != null)
                {
                    state.CurrentStep = nextSagaStep.StepName;
                }
                else
                {
                    state.CurrentStep = null;
                }

                await sagaPersistance.
                    Set(state);

                if (prevState != state.CurrentState)
                    internalMessageBus.Publish(
                        new SagaStateChangedMessage(typeof(TSagaState), state.CorrelationID, state.CurrentState, state.CurrentStep, state.IsCompensating));

                internalMessageBus.Publish(
                    new SagaStepChangedMessage(typeof(TSagaState), state.CorrelationID, state.CurrentState, state.CurrentStep, state.IsCompensating));

                if (sagaStep.Async)
                {
                    internalMessageBus.Publish(
                        new SagaStepCompletedAsyncMessage(typeof(TSagaState), state.CorrelationID, state.CurrentState, state.CurrentStep, state.IsCompensating));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
    internal class StepExecutionResult
    {
        internal bool Async;

        internal ISagaState State;
    }
}