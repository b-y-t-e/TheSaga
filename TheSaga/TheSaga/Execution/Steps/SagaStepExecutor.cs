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

namespace TheSaga.Execution.Steps
{
    internal class SagaStepExecutor<TSagaState>
        where TSagaState : ISagaState
    {
        private IEvent @event;
        private ISagaAction action;
        private bool @async;
        private IInternalMessageBus internalMessageBus;
        private ISagaPersistance sagaPersistance;
        private ISagaState state;

        public SagaStepExecutor(
            Boolean async,
            IEvent @event,
            ISagaState state,
            ISagaAction action,
            IInternalMessageBus internalMessageBus,
            ISagaPersistance sagaPersistance)
        {
            this.@async = async;
            this.@event = @event;
            this.internalMessageBus = internalMessageBus;
            this.sagaPersistance = sagaPersistance;
            this.state = state;
            this.action = action;
        }

        internal async Task Execute()
        {
            if (@async)
            {
                ExecuteStepAsync();
            }
            else
            {
                await ExecuteStepSync();
            }
        }

        private void ExecuteStepAsync()
        {
            Task.Run(() => ExecuteStepSync());
        }

        private async Task ExecuteStepSync()
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
}