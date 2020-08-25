using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Events;
using TheSaga.Execution.Context;
using TheSaga.Persistance;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;
using TheSaga.States;

namespace TheSaga.Execution
{
    internal class SagaStepExecutor<TSagaState>
        where TSagaState : ISagaState
    {
        ISagaPersistance sagaPersistance;
        IEvent @event;
        ISagaState state;
        ISagaAction action;

        public SagaStepExecutor(
            ISagaPersistance sagaPersistance, IEvent @event, ISagaState state, ISagaAction action)
        {
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

                await sagaPersistance.Set(state);

                if(sagaStep.Async)
                {
                    // zalowanie kolejnego kroku
                    throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}