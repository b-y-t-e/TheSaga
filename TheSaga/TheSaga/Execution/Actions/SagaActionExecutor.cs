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
        private ISagaAction action;
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

        internal async Task<ActionExecutionResult> ExecuteStep()
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
                    return new ActionExecutionResult()
                    {
                        State = state,
                        IsProcessingComplete = async || state?.CurrentStep == null
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

            await new SagaStepExecutor<TSagaState>(async, @event, state, action, internalMessageBus, sagaPersistance).
                Execute();

            return new ActionExecutionResult()
            {
                State = state,
                IsProcessingComplete = async || state?.CurrentStep == null
            };
        }
    }
    internal class ActionExecutionResult
    {
        internal bool IsProcessingComplete;

        internal ISagaState State;
    }
}