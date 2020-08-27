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
    internal class SagaStepExecutor<TSagaData>
        where TSagaData : ISagaData
    {
        private ISaga saga;
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
            ISaga saga,
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
            this.sagaStep = sagaStep;
            this.sagaAction = sagaAction;
            this.dateTimeProvider = dateTimeProvider;
            this.saga = saga;
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
            string currentState = saga.State.CurrentState;
            bool hasSagaCompleted = false;
            ISagaStep nextSagaStep = FindNextStep();

            if (!saga.State.IsCompensating &&
                sagaStep == sagaAction.Steps.First())
            {
                saga.State.CurrentError = null;
            }

            saga.State.CurrentStep = sagaStep.StepName;
            saga.Info.Modified = dateTimeProvider.Now;

            SagaStepHistory stepLog = CreateStepLog(nextSagaStep);

            await sagaPersistance.
                Set(saga);

            try
            {
#if DEBUG
                Console.WriteLine($"state: {saga.State.CurrentState}; step: {saga.State.CurrentStep}; action: {(saga.State.IsCompensating ? "Compensate" : "Execute")}");
#endif

                IExecutionContext context = new ExecutionContext<TSagaData>((TSagaData)saga.Data, saga.Info, saga.State);

                if (@event is EmptyEvent)
                    @event = null;

                if (saga.State.IsCompensating)
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
                saga.State.IsCompensating = true;
                saga.State.CurrentError = ex.ToSagaStepException();

                stepLog.HasSucceeded = false;
                stepLog.Error = ex.ToSagaStepException();
            }
            finally
            {
                stepLog.HasFinished = true;
            }

            if (nextSagaStep != null)
            {
                saga.State.CurrentStep = nextSagaStep.StepName;
            }
            else
            {
                hasSagaCompleted = true;
                saga.State.IsCompensating = false;
                saga.State.CurrentStep = null;
            }
            saga.Info.Modified = dateTimeProvider.Now;

            await sagaPersistance.
                Set(saga);

            SendInternalMessages(currentState, hasSagaCompleted);

            ThrowErrorIfSagaCompletedForSyncCall();
        }

        private ISagaStep FindNextStep()
        {
            ISagaStep nextSagaStep = null;
            if (saga.State.IsCompensating)
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
                StateName = saga.State.CurrentState,
                StepName = saga.State.CurrentStep,
                IsCompensating = saga.State.IsCompensating,
                Async = @async,
                NextStepName = nextSagaStep == null ? null : nextSagaStep.StepName
            };
            saga.Info.History.Add(stepLog);
            return stepLog;
        }

        private void ThrowErrorIfSagaCompletedForSyncCall()
        {
            if (!@async)
            {
                if (saga.IsProcessingCompleted() &&
                    saga.State.CurrentError != null)
                {
                    throw saga.State.CurrentError;
                }
            }
        }

        private void SendInternalMessages(string currentState, bool hasSagaCompleted)
        {
            if (currentState != saga.State.CurrentState)
            {
                internalMessageBus.Publish(
                    new SagaStateChangedMessage(typeof(TSagaData), saga.Data.CorrelationID, saga.State.CurrentState, saga.State.CurrentStep, saga.State.IsCompensating));
            }

            if (hasSagaCompleted)
            {
                internalMessageBus.Publish(
                    new SagaProcessingCompletedMessage(typeof(TSagaData), saga.Data.CorrelationID));
            }
            else
            {
                if (@async)
                {
                    internalMessageBus.Publish(
                        new SagaAsyncStepCompletedMessage(typeof(TSagaData), saga.Data.CorrelationID, saga.State.CurrentState, saga.State.CurrentStep, saga.State.IsCompensating));
                }
            }
        }
    }
}