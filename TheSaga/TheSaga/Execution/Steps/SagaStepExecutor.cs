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
            string currentState = sagaState.SagaState.SagaCurrentState;
            bool hasSagaCompleted = false;
            ISagaStep nextSagaStep = FindNextStep();

            if (!sagaState.SagaState.SagaIsCompensating &&
                sagaStep == sagaAction.Steps.First())
            {
                sagaState.SagaState.SagaCurrentError = null;
            }

            sagaState.SagaState.SagaCurrentStep = sagaStep.StepName;
            sagaState.SagaInfo.SagaModified = dateTimeProvider.Now;

            SagaStepHistory stepLog = CreateStepLog(nextSagaStep);

            await sagaPersistance.
                Set(sagaState);

            try
            {
#if DEBUG
                Console.WriteLine($"state: {sagaState.SagaState.SagaCurrentState}; step: {sagaState.SagaState.SagaCurrentStep}; action: {(sagaState.SagaState.SagaIsCompensating ? "Compensate" : "Execute")}");
#endif

                IExecutionContext context = new ExecutionContext<TSagaState>()
                {
                    State = (TSagaState)sagaState
                };

                if (@event is EmptyEvent)
                    @event = null;

                if (sagaState.SagaState.SagaIsCompensating)
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
                sagaState.SagaState.SagaIsCompensating = true;
                sagaState.SagaState.SagaCurrentError = ex.ToSagaStepException();

                stepLog.HasSucceeded = false;
                stepLog.Error = ex.ToSagaStepException();
            }
            finally
            {
                stepLog.HasFinished = true;
            }

            if (nextSagaStep != null)
            {
                sagaState.SagaState.SagaCurrentStep = nextSagaStep.StepName;
            }
            else
            {
                hasSagaCompleted = true;
                sagaState.SagaState.SagaIsCompensating = false;
                sagaState.SagaState.SagaCurrentStep = null;
            }
            sagaState.SagaInfo.SagaModified = dateTimeProvider.Now;

            await sagaPersistance.
                Set(sagaState);

            SendInternalMessages(currentState, hasSagaCompleted);

            ThrowErrorIfSagaCompletedForSyncCall();
        }

        private ISagaStep FindNextStep()
        {
            ISagaStep nextSagaStep = null;
            if (sagaState.SagaState.SagaIsCompensating)
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
                StateName = sagaState.SagaState.SagaCurrentState,
                StepName = sagaState.SagaState.SagaCurrentStep,
                IsCompensating = sagaState.SagaState.SagaIsCompensating,
                Async = @async,
                NextStepName = nextSagaStep == null ? null : nextSagaStep.StepName
            };
            sagaState.SagaInfo.SagaHistory.Add(stepLog);
            return stepLog;
        }

        private void ThrowErrorIfSagaCompletedForSyncCall()
        {
            if (!@async)
            {
                if (sagaState.IsProcessingCompleted() &&
                    sagaState.SagaState.SagaCurrentError != null)
                {
                    throw sagaState.SagaState.SagaCurrentError;
                }
            }
        }

        private void SendInternalMessages(string currentState, bool hasSagaCompleted)
        {
            if (currentState != sagaState.SagaState.SagaCurrentState)
            {
                internalMessageBus.Publish(
                    new SagaStateChangedMessage(typeof(TSagaState), sagaState.CorrelationID, sagaState.SagaState.SagaCurrentState, sagaState.SagaState.SagaCurrentStep, sagaState.SagaState.SagaIsCompensating));
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
                        new SagaAsyncStepCompletedMessage(typeof(TSagaState), sagaState.CorrelationID, sagaState.SagaState.SagaCurrentState, sagaState.SagaState.SagaCurrentStep, sagaState.SagaState.SagaIsCompensating));
                }
            }
        }
    }
}