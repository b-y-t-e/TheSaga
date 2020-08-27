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

        public ISaga Saga;
    }

    internal class SagaActionExecutor<TSagaData> : ISagaActionExecutor<TSagaData>
        where TSagaData : ISagaData
    {
        private IsExecutionAsync @async;
        private IEvent @event;
        private Guid id;
        private ISagaModel<TSagaData> model;
        private ISagaPersistance sagaPersistance;
        private IServiceProvider serviceProvider;
        private ISaga saga;

        public SagaActionExecutor(
            Guid id,
            IsExecutionAsync async,
            IEvent @event,
            ISagaModel<TSagaData> model,
            ISagaPersistance sagaPersistance,
            IServiceProvider serviceProvider)
        {
            this.id = id;
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

            this.saga = await sagaPersistance.
                Get(id);

            if (saga == null)
                throw new SagaInstanceNotFoundException(model.SagaStateType, id);

            IList<ISagaAction> actions = model.
                FindActionsForState(saga.State.CurrentState);

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
               CreateInstance<SagaStepExecutor<TSagaData>>(serviceProvider, async, @event, saga, step, action);

            await stepExecutor.ExecuteStep();

            return new ActionExecutionResult()
            {
                Saga = saga,
                IsSyncProcessingComplete = async || saga.IsProcessingCompleted()
            };
        }

        private ISagaStep FindStepForCurrentState(IList<ISagaAction> actions)
        {
            if (saga.IsProcessingCompleted())
            {
                throw new Exception("");
            }

            ISagaAction action = actions.
                FirstOrDefault(a => a.FindStep(saga.State.CurrentStep) != null);

            if (action == null)
                throw new SagaStepNotRegisteredException(saga.State.CurrentState, saga.State.CurrentStep);

            ISagaStep step = action.
                FindStep(saga.State.CurrentStep);

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.State.CurrentState, saga.State.CurrentStep);

            return step;
        }

        private ISagaStep FindStepForEventType(Type eventType, IList<ISagaAction> actions)
        {
            ISagaAction action = actions.
                FirstOrDefault(a => a.Event == eventType);

            if (action == null)
                throw new SagaInvalidEventForStateException(saga.State.CurrentState, eventType);

            if (!saga.IsProcessingCompleted())
                throw new SagaIsBusyHandlingStepException(saga.Data.ID, saga.State.CurrentState, saga.State.CurrentStep);

            ISagaStep step = action.Steps.FirstOrDefault();

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.State.CurrentState, saga.State.CurrentStep);

            return step;
        }
    }
}