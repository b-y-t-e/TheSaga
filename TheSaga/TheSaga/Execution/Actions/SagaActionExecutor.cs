using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Execution.Context;
using TheSaga.Execution.Steps;
using TheSaga.Messages;
using TheSaga.Messages.MessageBus;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;
using TheSaga.Utils;

namespace TheSaga.Execution.Actions
{
    internal class SagaActionExecutor<TSagaState>
        where TSagaState : ISagaState
    {
        private IEvent @event;
        private Guid correlationID;
        private bool @async;
        private IInternalMessageBus internalMessageBus;
        private ISagaModel<TSagaState> model;
        private ISagaPersistance sagaPersistance;
        private ISagaState state;

        public SagaActionExecutor(
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

        internal async Task<ActionExecutionResult> ExecuteAction()
        {
            Type eventType = @event == null ?
                null : @event.GetType();

            this.state = await sagaPersistance.
                Get(correlationID);

            if (state == null)
                throw new SagaInstanceNotFoundException(model.SagaStateType, correlationID);

            IList<ISagaAction> actions = model.
                FindActionsForState(state.CurrentState);

            ISagaStep step = null;
            if (eventType != null)
            {
                step = FindStepForEventType(eventType, actions);
            }
            else
            {
                step = FindStepForCurrentState(actions);
            }

            ISagaAction action = model.
                FindActionForStep(step);

            if (step.Async)
                async = true;

            await new SagaStepExecutor<TSagaState>(async, @event, state, step, action, internalMessageBus, sagaPersistance).
                Execute();

            return new ActionExecutionResult()
            {
                State = state,
                IsSyncProcessingComplete = async || state.IsProcessingCompleted()
            };
        }

        private ISagaStep FindStepForCurrentState(IList<ISagaAction> actions)
        {
            if (state.IsProcessingCompleted())
            {
                throw new Exception("");
                /*return new ActionExecutionResult()
                {
                    State = state,
                    IsProcessingComplete = async || state?.CurrentStep == null
                };*/
            }

            ISagaAction action = actions.
                FirstOrDefault(a => a.FindStep(state.CurrentStep) != null);

            if (action == null)
                throw new SagaStepNotRegisteredException(state.CurrentState, state.CurrentStep);

            ISagaStep step = action.
                FindStep(state.CurrentStep);

            if (step == null)
                throw new SagaStepNotRegisteredException(state.CurrentState, state.CurrentStep);

            return step;
        }

        private ISagaStep FindStepForEventType(Type eventType, IList<ISagaAction> actions)
        {
            ISagaAction action = actions.
                                FirstOrDefault(a => a.Event == eventType);
            if (action == null)
                throw new SagaInvalidEventForStateException(state.CurrentState, eventType);

            if (!state.IsProcessingCompleted())
                throw new SagaIsBusyHandlingStepException(state.CorrelationID, state.CurrentState, state.CurrentStep);

            ISagaStep step = action.Steps.FirstOrDefault();

            if (step == null)
                throw new SagaStepNotRegisteredException(state.CurrentState, state.CurrentStep);

            return step;
        }
    }
    internal class ActionExecutionResult
    {
        internal bool IsSyncProcessingComplete;

        internal ISagaState State;
    }
}