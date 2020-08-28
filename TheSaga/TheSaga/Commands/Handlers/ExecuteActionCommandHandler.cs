using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.Steps;
using TheSaga.Utils;
using TheSaga.ValueObjects;

namespace TheSaga.Commands.Handlers
{
    internal class ExecuteActionCommandHandler
    {
        private readonly ISagaPersistance sagaPersistance;
        private IServiceProvider serviceProvider;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public ExecuteActionCommandHandler(
            ISagaPersistance sagaPersistance,
            IServiceProvider serviceProvider,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.sagaPersistance = sagaPersistance;
            this.serviceProvider = serviceProvider;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<ExecuteActionResult> Handle(ExecuteActionCommand command)
        {
            if (command.Event == null)
                command.Event = new EmptyEvent();

            var saga = await sagaPersistance.Get(command.ID);

            if (saga == null)
                throw new SagaInstanceNotFoundException(command.Model.SagaStateType, command.ID);

            var actions = command.Model.FindActionsForState(saga.State.GetExecutionState());
            var step = FindStep(saga, command.Event.GetType(), actions);
            var action = command.Model.FindActionForStep(step);

            var async = AsyncExecution.From(step.Async);
            if (step.Async)
                async = AsyncExecution.True();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var stepExecutor = ActivatorUtilities.CreateInstance<ExecuteStepCommandHandler>(scope.ServiceProvider);

                var sagaFinalState = await stepExecutor.Handle(new ExecuteStepCommand
                {
                    Async = async,
                    Event = command.Event,
                    Saga = saga,
                    SagaStep = step,
                    SagaAction = action,
                    Model = command.Model
                });

                return new ExecuteActionResult
                {
                    Saga = sagaFinalState ?? saga,
                    IsSyncProcessingComplete = sagaFinalState == null || async || saga.IsIdle()
                };
            }
        }

        private ISagaStep FindStep(ISaga saga, Type eventType, IList<ISagaAction> actions)
        {
            if (!eventType.Is<EmptyEvent>())
                return FindStepForEventType(saga, eventType, actions);
            return FindStepForCurrentState(saga, actions);
        }

        private ISagaStep FindStepForCurrentState(ISaga saga, IList<ISagaAction> actions)
        {
            if (saga.IsIdle())
                throw new Exception("");

            var action = actions.FirstOrDefault(a => a.FindStep(saga.State.CurrentStep) != null);

            if (action == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            var step = action.FindStep(saga.State.CurrentStep);

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            return step;
        }

        private ISagaStep FindStepForEventType(ISaga saga, Type eventType, IList<ISagaAction> actions)
        {
            var action = actions.FirstOrDefault(a => a.Event == eventType);

            if (action == null)
                throw new SagaInvalidEventForStateException(saga.State.GetExecutionState(), eventType);

            if (!saga.IsIdle())
                throw new SagaIsBusyHandlingStepException(saga.Data.ID, saga.State.GetExecutionState(),
                    saga.State.CurrentStep);

            var step = action.Steps.FirstOrDefault();

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            return step;
        }
    }
}