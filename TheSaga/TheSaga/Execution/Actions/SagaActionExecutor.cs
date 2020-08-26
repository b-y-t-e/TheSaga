using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Execution.Steps;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Providers;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;
using TheSaga.Utils;

namespace TheSaga.Execution.Actions
{
    internal class ActionExecutionResult
    {
        public bool IsSyncProcessingComplete;

        public ISagaData State;
    }

    internal class SagaActionExecutor<TSagaData> : ISagaActionExecutor<TSagaData>
        where TSagaData : ISagaData
    {
        private IsExecutionAsync @async;
        private IEvent @event;
        private Guid correlationID;
        private ISagaModel<TSagaData> model;
        private ISagaPersistance sagaPersistance;
        private IServiceProvider serviceProvider;
        private ISagaData state;

        public SagaActionExecutor(
            Guid correlationID,
            IsExecutionAsync async,
            IEvent @event,
            ISagaModel<TSagaData> model,
            ISagaPersistance sagaPersistance,
            IServiceProvider serviceProvider)
        {
            this.correlationID = correlationID;
            this.@async = async;
            this.@event = @event;
            this.model = model;
            this.sagaPersistance = sagaPersistance;
            this.serviceProvider = serviceProvider;
        }

        public async Task<ActionExecutionResult> ExecuteAction()
        {
            if (@event == null)
                @event = new EmptyEvent();

            Type eventType = @event.GetType();

            this.state = await sagaPersistance.
                Get(correlationID);

            if (state == null)
                throw new SagaInstanceNotFoundException(model.SagaStateType, correlationID);

            IList<ISagaAction> actions = model.
                FindActionsForState(state.SagaState.CurrentState);

            ISagaStep step = null;
            if (!eventType.Is<EmptyEvent>())
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
                async = IsExecutionAsync.True();

            //await new SagaStepExecutor<TSagaData>(async, @event, state, step, action, internalMessageBus, sagaPersistance, dateTimeProvider).
            //ExecuteStep();

            SagaStepExecutor<TSagaData> stepExecutor = ActivatorUtilities.
               CreateInstance<SagaStepExecutor<TSagaData>>(serviceProvider, async, @event, state, step, action);

            await stepExecutor.ExecuteStep();

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
            }

            ISagaAction action = actions.
                FirstOrDefault(a => a.FindStep(state.SagaState.CurrentStep) != null);

            if (action == null)
                throw new SagaStepNotRegisteredException(state.SagaState.CurrentState, state.SagaState.CurrentStep);

            ISagaStep step = action.
                FindStep(state.SagaState.CurrentStep);

            if (step == null)
                throw new SagaStepNotRegisteredException(state.SagaState.CurrentState, state.SagaState.CurrentStep);

            return step;
        }

        private ISagaStep FindStepForEventType(Type eventType, IList<ISagaAction> actions)
        {
            ISagaAction action = actions.
                FirstOrDefault(a => a.Event == eventType);

            if (action == null)
                throw new SagaInvalidEventForStateException(state.SagaState.CurrentState, eventType);

            if (!state.IsProcessingCompleted())
                throw new SagaIsBusyHandlingStepException(state.CorrelationID, state.SagaState.CurrentState, state.SagaState.CurrentStep);

            ISagaStep step = action.Steps.FirstOrDefault();

            if (step == null)
                throw new SagaStepNotRegisteredException(state.SagaState.CurrentState, state.SagaState.CurrentStep);

            return step;
        }
    }
}