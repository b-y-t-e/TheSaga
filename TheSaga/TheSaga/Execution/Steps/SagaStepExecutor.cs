using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Execution.Context;
using TheSaga.InternalMessages;
using TheSaga.InternalMessages.MessageBus;
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
        private ISagaState sagaState;
        private ISagaAction sagaAction;
        private ISagaStep sagaStep;
        private IEvent @event;
        private bool @async;
        private IInternalMessageBus internalMessageBus;
        private ISagaPersistance sagaPersistance;

        public SagaStepExecutor(
            Boolean async,
            IEvent @event,
            ISagaState state,
            ISagaStep sagaStep,
            ISagaAction sagaAction,
            IInternalMessageBus internalMessageBus,
            ISagaPersistance sagaPersistance)
        {
            this.@async = async;
            this.@event = @event;
            this.internalMessageBus = internalMessageBus;
            this.sagaPersistance = sagaPersistance;
            this.sagaState = state;
            this.sagaStep = sagaStep;
            this.sagaAction = sagaAction;
        }

        internal async Task ExecuteStep()
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
            string currentState = sagaState.CurrentState;
            bool hasSagaCompleted = false;
            ISagaStep nextSagaStep = FindNextStep();

            if (!sagaState.IsCompensating &&
                sagaStep == sagaAction.Steps.First())
            {
                sagaState.CurrentError = null;
            }

            sagaState.CurrentStep = sagaStep.StepName;

            SagaStepHistory stepLog = CreateStepLog(nextSagaStep);

            await sagaPersistance.
                Set(sagaState);

            try
            {
#if DEBUG
                Console.WriteLine($"state: {sagaState.CurrentState}; step: {sagaState.CurrentStep}; action: {(sagaState.IsCompensating ? "Compensate" : "Execute")}");
#endif

                IExecutionContext context = new ExecutionContext<TSagaState>()
                {
                    State = (TSagaState)sagaState
                };

                if (sagaState.IsCompensating)
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
                sagaState.IsCompensating = true;
                sagaState.CurrentError = ex.ToSagaStepException();

                stepLog.HasSucceeded = false;
                stepLog.Error = ex.ToSagaStepException();
            }
            finally
            {
                stepLog.HasFinished = true;
            }

            if (nextSagaStep != null)
            {
                sagaState.CurrentStep = nextSagaStep.StepName;
            }
            else
            {
                hasSagaCompleted = true;
                sagaState.IsCompensating = false;
                sagaState.CurrentStep = null;
            }

            await sagaPersistance.
                Set(sagaState);

            SendInternalMessages(currentState, hasSagaCompleted);

            ThrowErrorIfSagaCompletedForSyncCall();
        }

        private ISagaStep FindNextStep()
        {
            ISagaStep nextSagaStep = null;
            if (sagaState.IsCompensating)
            {
                nextSagaStep = sagaAction.FindPrevBefore(sagaStep);
            }
            else
            {
                nextSagaStep = sagaAction.FindNextAfter(sagaStep);
            }

            return nextSagaStep;
        }

        private SagaStepHistory CreateStepLog(ISagaStep nextSagaStep)
        {
            SagaStepHistory stepLog = new SagaStepHistory()
            {
                Created = DateTime.Now,
                StateName = sagaState.CurrentState,
                StepName = sagaState.CurrentStep,
                IsCompensating = sagaState.IsCompensating,
                Async = @async,
                NextStepName = nextSagaStep == null ? null : nextSagaStep.StepName
            };
            sagaState.History.Add(stepLog);
            return stepLog;
        }

        private void ThrowErrorIfSagaCompletedForSyncCall()
        {
            if (!@async)
            {
                if (sagaState.IsProcessingCompleted() &&
                    sagaState.CurrentError != null)
                {
                    throw sagaState.CurrentError;
                }
            }
        }

        private void SendInternalMessages(string currentState, bool hasSagaCompleted)
        {
            if (currentState != sagaState.CurrentState)
            {
                internalMessageBus.Publish(
                    new SagaStateChangedMessage(typeof(TSagaState), sagaState.CorrelationID, sagaState.CurrentState, sagaState.CurrentStep, sagaState.IsCompensating));
            }

            if (hasSagaCompleted)
            {
                internalMessageBus.Publish(
                    new SagaProcessingCompletedMessage(typeof(TSagaState), sagaState.CorrelationID));
            }
            else
            {
                if (@async)
                {
                    internalMessageBus.Publish(
                        new SagaAsyncStepCompletedMessage(typeof(TSagaState), sagaState.CorrelationID, sagaState.CurrentState, sagaState.CurrentStep, sagaState.IsCompensating));
                }
            }
        }
    }
}