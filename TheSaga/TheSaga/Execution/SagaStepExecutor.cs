using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Execution.Context;
using TheSaga.Messages;
using TheSaga.Messages.MessageBus;
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
        private IInternalMessageBus internalMessageBus;
        private ISagaPersistance sagaPersistance;
        private ISagaState state;

        public SagaStepExecutor(
            IInternalMessageBus internalMessageBus,
            ISagaPersistance sagaPersistance,
            IEvent @event,
            ISagaState state,
            ISagaAction action)
        {
            this.internalMessageBus = internalMessageBus;
            this.sagaPersistance = sagaPersistance;
            this.@event = @event;
            this.state = state;
            this.action = action;
        }

        public async Task<bool> Run()
        {
            ISagaStep step = action.
                FindStep(state.CurrentStep);

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
                        new SagaAsyncStepCompletedMessage(typeof(TSagaState), state.CorrelationID, state.CurrentState, state.CurrentStep, state.IsCompensating));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}