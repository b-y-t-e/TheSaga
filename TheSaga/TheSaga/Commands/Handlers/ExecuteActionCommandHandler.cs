using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.MessageBus;
using TheSaga.Messages;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.SagaModels;
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
        private IServiceProvider serviceProvider;
        private IMessageBus messageBus;

        public ExecuteActionCommandHandler(
            ISagaPersistance sagaPersistance,
            IServiceProvider serviceProvider,
            IServiceScopeFactory serviceScopeFactory,
            IMessageBus messageBus)
        {
            this.sagaPersistance = sagaPersistance;
            this.serviceProvider = serviceProvider;
            this.serviceScopeFactory = serviceScopeFactory;
            this.messageBus = messageBus;
        }

        public async Task<ExecuteActionResult> Handle(ExecuteActionCommand command)
        {
            if (command.Event == null)
                command.Event = new EmptyEvent();

            ISaga saga = await sagaPersistance.Get(command.ID);

            if (saga == null)
                throw new SagaInstanceNotFoundException(command.Model.SagaStateType, command.ID);

            IList<ISagaAction> actions = command.Model.FindActionsForState(saga.State.GetExecutionState());
            ISagaStep step = FindStep(saga, command.Event.GetType(), actions);
            ISagaAction action = command.Model.FindActionForStep(step);

            if (step.Async)
                saga.State.AsyncExecution = AsyncExecution.True();

            ExecuteStepCommand executeStepCommand = new ExecuteStepCommand
            {
                AsyncExecution = saga.State.AsyncExecution,
                Event = command.Event,
                Saga = saga,
                SagaStep = step,
                SagaAction = action,
                Model = command.Model
            };

            if (step.Async)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await DispatchStepAsync(saga, executeStepCommand);
                    }
                    catch (Exception ex)
                    {

                    }
                });
                return new ExecuteActionResult()
                {
                    IsSyncProcessingComplete = true,
                    Saga = saga
                };
            }
            else
            {
                return await DispatchStepSync(saga, executeStepCommand);
            }

        }

        async Task<ExecuteActionResult> DispatchStepSync(
            ISaga saga, ExecuteStepCommand executeStepCommand)
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                ExecuteStepCommandHandler stepExecutor = ActivatorUtilities.
                    CreateInstance<ExecuteStepCommandHandler>(scope.ServiceProvider);

                saga = await stepExecutor.
                    Handle(executeStepCommand);

                if (saga == null)
                {
                    await messageBus.Publish(
                        new ExecutionEndMessage(saga));

                    return new ExecuteActionResult()
                    {
                        Saga = null,
                        IsSyncProcessingComplete = true
                    };
                }
                else
                {
                    if (saga.IsIdle())
                        await messageBus.Publish(
                            new ExecutionEndMessage(saga));

                    if (saga.IsIdle() && saga.HasError())
                        throw saga.State.CurrentError;

                    return new ExecuteActionResult()
                    {
                        Saga = saga,
                        IsSyncProcessingComplete = saga.IsIdle()
                    };
                }
            }
        }

        async Task<ExecuteActionResult> DispatchStepAsync(
          ISaga saga, ExecuteStepCommand executeStepCommand)
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                ExecuteStepCommandHandler stepExecutor = ActivatorUtilities.
                    CreateInstance<ExecuteStepCommandHandler>(scope.ServiceProvider);

                saga = await stepExecutor.
                    Handle(executeStepCommand);

                if (saga == null)
                {
                    await messageBus.Publish(
                        new ExecutionEndMessage(saga));

                    return new ExecuteActionResult()
                    {
                        Saga = null,
                        IsSyncProcessingComplete = true
                    };
                }
                else
                {
                    if (saga.IsIdle())
                        await messageBus.Publish(
                            new ExecutionEndMessage(saga));

                    // command handler?
                    await messageBus.Publish(
                        new AsyncStepCompletedMessage(
                            SagaID.From(saga.Data.ID),
                            saga.State.CurrentState,
                            saga.State.CurrentStep,
                            saga.State.IsCompensating,
                            executeStepCommand.Model));

                    return new ExecuteActionResult()
                    {
                        IsSyncProcessingComplete = true,
                        Saga = saga
                    };
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
