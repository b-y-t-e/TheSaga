using System;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Execution.Actions;
using TheSaga.Execution.Context;
using TheSaga.InternalMessages;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Persistance;
using TheSaga.Providers;
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
        private IsExecutionAsync @async;
        private IInternalMessageBus internalMessageBus;
        private ISagaPersistance sagaPersistance;
        private IDateTimeProvider dateTimeProvider;

        public SagaStepExecutor(
            IsExecutionAsync async,
            IEvent @event,
            ISagaState state,
            ISagaStep sagaStep,
            ISagaAction sagaAction,
            IInternalMessageBus internalMessageBus,
            ISagaPersistance sagaPersistance, 
            IDateTimeProvider dateTimeProvider)
        {
            this.@async = async;
            this.@event = @event;
            this.internalMessageBus = internalMessageBus;
            this.sagaPersistance = sagaPersistance;
            this.sagaState = state;
            this.sagaStep = sagaStep;
            this.sagaAction = sagaAction;
            this.dateTimeProvider = dateTimeProvider;
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
            string currentState = sagaState.SagaCurrentState;
            bool hasSagaCompleted = false;
            ISagaStep nextSagaStep = FindNextStep();

            if (!sagaState.SagaIsCompensating &&
                sagaStep == sagaAction.Steps.First())
            {
                sagaState.SagaCurrentError = null;
            }

            sagaState.SagaCurrentStep = sagaStep.StepName;
            sagaState.SagaModified = dateTimeProvider.Now;

            SagaStepHistory stepLog = CreateStepLog(nextSagaStep);

            await sagaPersistance.
                Set(sagaState);

            try
            {
#if DEBUG
                Console.WriteLine($"state: {sagaState.SagaCurrentState}; step: {sagaState.SagaCurrentStep}; action: {(sagaState.SagaIsCompensating ? "Compensate" : "Execute")}");
#endif

                IExecutionContext context = new ExecutionContext<TSagaState>()
                {
                    State = (TSagaState)sagaState
                };

                if (@event is EmptyEvent)
                    @event = null;

                if (sagaState.SagaIsCompensating)
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
                sagaState.SagaIsCompensating = true;
                sagaState.SagaCurrentError = ex.ToSagaStepException();

                stepLog.HasSucceeded = false;
                stepLog.Error = ex.ToSagaStepException();
            }
            finally
            {
                stepLog.HasFinished = true;
            }

            if (nextSagaStep != null)
            {
                sagaState.SagaCurrentStep = nextSagaStep.StepName;
            }
            else
            {
                hasSagaCompleted = true;
                sagaState.SagaIsCompensating = false;
                sagaState.SagaCurrentStep = null;
            }
            sagaState.SagaModified = dateTimeProvider.Now;

            await sagaPersistance.
                Set(sagaState);

            SendInternalMessages(currentState, hasSagaCompleted);

            ThrowErrorIfSagaCompletedForSyncCall();
        }

        private ISagaStep FindNextStep()
        {
            ISagaStep nextSagaStep = null;
            if (sagaState.SagaIsCompensating)
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
                Created = dateTimeProvider.Now,
                StateName = sagaState.SagaCurrentState,
                StepName = sagaState.SagaCurrentStep,
                IsCompensating = sagaState.SagaIsCompensating,
                Async = @async,
                NextStepName = nextSagaStep == null ? null : nextSagaStep.StepName
            };
            sagaState.SagaHistory.Add(stepLog);
            return stepLog;
        }

        private void ThrowErrorIfSagaCompletedForSyncCall()
        {
            if (!@async)
            {
                if (sagaState.IsProcessingCompleted() &&
                    sagaState.SagaCurrentError != null)
                {
                    throw sagaState.SagaCurrentError;
                }
            }
        }

        private void SendInternalMessages(string currentState, bool hasSagaCompleted)
        {
            if (currentState != sagaState.SagaCurrentState)
            {
                internalMessageBus.Publish(
                    new SagaStateChangedMessage(typeof(TSagaState), sagaState.CorrelationID, sagaState.SagaCurrentState, sagaState.SagaCurrentStep, sagaState.SagaIsCompensating));
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
                        new SagaAsyncStepCompletedMessage(typeof(TSagaState), sagaState.CorrelationID, sagaState.SagaCurrentState, sagaState.SagaCurrentStep, sagaState.SagaIsCompensating));
                }
            }
        }
    }
}