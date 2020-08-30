using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Errors;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.MessageBus;
using TheSaga.Messages;
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
        private readonly IServiceScopeFactory serviceScopeFactory;
        private IMessageBus messageBus;
        private IAsyncSagaErrorHandler asyncErrorHandler;

        public ExecuteActionCommandHandler(
            ISagaPersistance sagaPersistance,
            IServiceScopeFactory serviceScopeFactory,
            IMessageBus messageBus, IAsyncSagaErrorHandler errorHandler)
        {
            this.sagaPersistance = sagaPersistance;
            this.serviceScopeFactory = serviceScopeFactory;
            this.messageBus = messageBus;
            this.asyncErrorHandler = errorHandler;
        }

        public async Task<ISaga> Handle(ExecuteActionCommand command)
        {
            if (command.Event == null)
                command.Event = new EmptyEvent();

            ISaga saga = command.Saga; // await sagaPersistance.Get(command.ID);

            if (saga == null)
                throw new SagaInstanceNotFoundException(command.Model.SagaStateType);

            IList<ISagaAction> actions = command.Model.
                FindActionsForState(saga.State.GetExecutionState());

            ISagaStep step = FindStep(saga, command.Event.GetType(), actions);

            ISagaAction action = command.Model.
                FindActionForStep(step);

            if (step.Async)
                saga.State.AsyncExecution = AsyncExecution.True();

            ExecuteStepCommand executeStepCommand = new ExecuteStepCommand
            {
                Event = command.Event,
                Saga = saga,
                SagaStep = step,
                SagaAction = action,
                Model = command.Model
            };

            if (step.Async)
            {
                DispatchStepAsync(executeStepCommand);
                return saga;
            }
            else
            {
                return await DispatchStepSync(executeStepCommand);
            }
        }

        private void DispatchStepAsync(
            ExecuteStepCommand command)
        {
            Task.Run(async () =>
            {
                try
                {
                    await DispatchStepSync(command);
                }
                catch (Exception ex)
                {
                    await asyncErrorHandler.Handle(command.Saga, ex);
                }
            });
        }

        private async Task<ISaga> DispatchStepSync(
            ExecuteStepCommand command)
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                ExecuteStepCommandHandler stepExecutor = ActivatorUtilities.
                    CreateInstance<ExecuteStepCommandHandler>(scope.ServiceProvider);

                ISaga saga = await stepExecutor.
                    Handle(command);

                if (saga == null)
                {
                    await messageBus.Publish(
                        new ExecutionEndMessage(saga));

                    return null;
                }
                else
                {
                    if (saga.IsIdle())
                    {
                        await messageBus.Publish(
                            new ExecutionEndMessage(saga));

                        if (saga.HasError())
                            throw saga.State.CurrentError;

                        return saga;
                    }
                    else
                    {
                        return await Handle(new ExecuteActionCommand()
                        {
                            Async = AsyncExecution.False(),
                            Event = new EmptyEvent(),
                            Saga = saga,
                            // ID = SagaID.From(saga.Data.ID),
                            Model = command.Model
                        });
                    }
                }
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

            ISagaAction action = actions.
                FirstOrDefault(a => a.FindStep(saga.State.CurrentStep) != null);

            if (action == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            ISagaStep step = action.FindStep(saga.State.CurrentStep);

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            return step;
        }

        private ISagaStep FindStepForEventType(ISaga saga, Type eventType, IList<ISagaAction> actions)
        {
            ISagaAction action = actions.FirstOrDefault(a => a.Event == eventType);

            if (action == null)
                throw new SagaInvalidEventForStateException(saga.State.GetExecutionState(), eventType);

            if (!saga.IsIdle())
                throw new SagaIsBusyHandlingStepException(saga.Data.ID, saga.State.GetExecutionState(),
                    saga.State.CurrentStep);

            ISagaStep step = action.Steps.FirstOrDefault();

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            return step;
        }
    }
}