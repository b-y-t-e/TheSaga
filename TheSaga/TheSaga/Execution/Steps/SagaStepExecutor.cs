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
            string currentSagaState = saga.State.CurrentState;
            bool hasSagaCompleted = false;

            if (!saga.State.IsCompensating &&
                sagaStep == sagaAction.Steps.First())
            {
                saga.State.CurrentError = null;
            }

            saga.State.CurrentStep = sagaStep.StepName;
            saga.Info.Modified = dateTimeProvider.Now;

            StepData executionData = saga.State.History.
                PrepareExecutionData(saga, async, dateTimeProvider);

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

                executionData.
                    Succeeded(dateTimeProvider);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                saga.State.IsCompensating = true;
                saga.State.CurrentError = ex.ToSagaStepException();

                executionData.
                    Failed(dateTimeProvider, ex.ToSagaStepException());
            }
            finally
            {
                executionData.
                    Ended(dateTimeProvider);
            }

            String nextStepName = CalculateNextStep();

            executionData.
                SetNextStepName(nextStepName);

            executionData.
                SetEndStateName(saga.State.CurrentState);

            if (nextStepName != null)
            {
                saga.State.CurrentStep = nextStepName;
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

            await SendInternalMessages(currentSagaState, hasSagaCompleted);

            ThrowErrorIfSagaCompletedForSyncCall();
        }

        private String CalculateNextStep()
        {
            String nextSagaStep = null;

            if (saga.State.IsCompensating)
            {
                StepData latestToCompensate = saga.State.History.
                    GetLatestToCompensate(saga.State.ExecutionID);

                if (latestToCompensate != null)
                    return latestToCompensate.StepName;

                return null;
            }
            else
            {
                nextSagaStep = sagaAction.
                    FindNextAfter(sagaStep)?.
                    StepName;
            }

            return nextSagaStep;
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

        private async Task SendInternalMessages(string currentState, bool hasSagaCompleted)
        {
            if (currentState != saga.State.CurrentState)
            {
                await internalMessageBus.Publish(
                    new SagaStateChangedMessage(typeof(TSagaData), SagaID.From(saga.Data.ID), saga.State.CurrentState, saga.State.CurrentStep, saga.State.IsCompensating));
            }

            if (hasSagaCompleted)
            {
                await internalMessageBus.Publish(
                    new SagaExecutionEndMessage(typeof(TSagaData), SagaID.From(saga.Data.ID)));
            }
            else
            {
                if (@async)
                {
                    await internalMessageBus.Publish(
                        new SagaAsyncStepCompletedMessage(typeof(TSagaData), SagaID.From(saga.Data.ID), saga.State.CurrentState, saga.State.CurrentStep, saga.State.IsCompensating));
                }
            }
        }
    }
}