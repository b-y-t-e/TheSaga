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

            ISaga saga = command.Saga;
            if (saga == null)
                throw new SagaInstanceNotFoundException(command.Model.SagaStateType);

            ISagaStep step = command.Model.Actions.
                FindStep(saga, command.Event.GetType());

            ISagaAction action = command.Model.
                FindActionForStep(step);

            if (step.Async)
                saga.ExecutionState.AsyncExecution = AsyncExecution.True();

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
                            throw saga.ExecutionState.CurrentError;

                        return saga;
                    }
                    else
                    {
                        return await Handle(new ExecuteActionCommand()
                        {
                            Async = AsyncExecution.False(),
                            Event = new EmptyEvent(),
                            Saga = saga,
                            Model = command.Model
                        });
                    }
                }
            }
        }

    }
}