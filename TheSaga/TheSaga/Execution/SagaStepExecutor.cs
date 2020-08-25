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
using TheSaga.Utils;

namespace TheSaga.Execution
{
    internal class SagaStepExecutor<TSagaState>
        where TSagaState : ISagaState
    {
        private IEvent @event;
        private ISagaAction action;
        private Guid correlationID;
        private bool @async;
        private IInternalMessageBus internalMessageBus;
        private ISagaModel<TSagaState> model;
        private ISagaPersistance sagaPersistance;
        private ISagaState state;

        public SagaStepExecutor(
            Guid correlationID,
            Boolean async,
            IEvent @event,
            ISagaModel<TSagaState> model,
            IInternalMessageBus internalMessageBus,
            ISagaPersistance sagaPersistance)
        {
            this.correlationID = correlationID;
            this.@async = async;
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
                if (state.CurrentStep == null)
                {
                    return new StepExecutionResult()
                    {
                        Async = false,
                        State = state
                    };
                }

                action = actions.
                    FirstOrDefault(a => a.FindStep(state.CurrentStep) != null);

                if (action == null)
                    throw new SagaStepNotRegisteredException(state.CurrentState, state.CurrentStep);
            }

            ISagaStep step = action.
                FindStep(state.CurrentStep);

            if (step == null)
                throw new SagaStepNotRegisteredException(state.CurrentState, state.CurrentStep);

            if (step.Async)
                async = true;

            await runStep();

            return new StepExecutionResult()
            {
                State = state,
                Async = @async
            };
        }

        private async Task<bool> runStep()
        {
            if (@async)
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
            string prevState = state.CurrentState;
            string prevStep = state.CurrentStep;

            ISagaStep sagaStep = action.
                FindStep(state.CurrentStep);

            ISagaStep nextSagaStep = null;
            if (state.IsCompensating)
            {
                nextSagaStep = action.FindPrevBefore(sagaStep);
            }
            else
            {
                nextSagaStep = action.FindNextAfter(sagaStep);
            }

            if (!state.IsCompensating &&
                sagaStep == action.Steps.First())
            {
                state.CurrentError = null;
            }

            SagaStepHistory stepLog = new SagaStepHistory()
            {
                Created = DateTime.Now,
                StateName = state.CurrentState,
                StepName = state.CurrentStep,
                IsCompensating = state.IsCompensating,
                Async = @async,
                NextStepName = nextSagaStep == null ? null : nextSagaStep.StepName
            };
            state.History.Add(stepLog);

            await sagaPersistance.
                Set(state);

            try
            {
#if DEBUG
                Console.WriteLine($"state: {state.CurrentState}; step: {state.CurrentStep}; action: {(state.IsCompensating ? "Compensate" : "Execute")}");
#endif

                IExecutionContext context = new ExecutionContext<TSagaState>()
                {
                    State = (TSagaState)state
                };

                if (state.IsCompensating)
                {
                    await sagaStep.
                        Compensate(context, @event);
                }
                else
                {
                    await sagaStep.
                        Execute(context, @event);
                }

                stepLog.HasSucceeded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                state.IsCompensating = true;
                state.CurrentError = ex.ToSagaStepException();

                stepLog.HasSucceeded = false;
                stepLog.Error = ex.ToSagaStepException();
            }
            finally
            {
                stepLog.HasFinished = true;
            }

            if (nextSagaStep != null)
            {
                state.CurrentStep = nextSagaStep.StepName;
            }
            else
            {
                state.IsCompensating = false;
                state.CurrentStep = null;
            }

            await sagaPersistance.
                Set(state);

            internalMessageBus.Publish(
                new SagaStepChangedMessage(typeof(TSagaState), state.CorrelationID, state.CurrentState, state.CurrentStep, state.IsCompensating));

            if (prevState != state.CurrentState)
            {
                internalMessageBus.Publish(
                    new SagaStateChangedMessage(typeof(TSagaState), state.CorrelationID, state.CurrentState, state.CurrentStep, state.IsCompensating));
            }

            if (@async)
            {
                internalMessageBus.Publish(
                    new SagaStepCompletedAsyncMessage(typeof(TSagaState), state.CorrelationID, state.CurrentState, state.CurrentStep, state.IsCompensating));
            }
            else
            {
                if (state.CurrentStep == null && state.CurrentError != null)
                    throw state.CurrentError;
            }
        }
    }
    internal class StepExecutionResult
    {
        internal bool Async;

        internal ISagaState State;
    }
}