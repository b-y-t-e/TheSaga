using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Errors;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.MessageBus;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Messages;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga;
using TheSaga.ModelsSaga.Actions;
using TheSaga.ModelsSaga.Actions.Interfaces;
using TheSaga.ModelsSaga.Steps.Interfaces;
using TheSaga.Persistance;
using TheSaga.States;
using TheSaga.Utils;
using TheSaga.ValueObjects;

namespace TheSaga.Commands.Handlers
{
    internal class ExecuteActionCommandHandler
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private IMessageBus messageBus;
        private IAsyncSagaErrorHandler asyncErrorHandler;
        private readonly ILogger logger;

        public ExecuteActionCommandHandler(
            IServiceScopeFactory serviceScopeFactory,
            IMessageBus messageBus,
            IAsyncSagaErrorHandler errorHandler,
            ILogger logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.messageBus = messageBus;
            this.asyncErrorHandler = errorHandler;
            this.logger = logger;
        }

        public async Task<ISaga> Handle(ExecuteActionCommand command)
        {
            ISaga saga = command.Saga;
            if (saga == null)
                throw new SagaInstanceNotFoundException(command.Model.SagaStateType);

            ISagaStep step = command.Model.Actions.
                FindStepForExecutionStateAndEvent(saga);

            ISagaAction action = command.Model.
                FindActionForStep(step);

            if (step.Async)
                saga.ExecutionState.AsyncExecution = AsyncExecution.True();

            ExecuteStepCommand executeStepCommand = new ExecuteStepCommand
            {
                Saga = saga,
                SagaStep = step,
                SagaAction = action,
                Model = command.Model
            };

            logger.
                LogDebug($"Saga: {saga.Data.ID}; Executing {(step.Async ? "async " : "")}step: {step.StepName}");

            if (step.Async)
            {
                DispatchStepAsync(executeStepCommand);
                return saga;
            }
            else
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    return await DispatchStepSync(scope.ServiceProvider, executeStepCommand);
                }
            }
        }

        private void DispatchStepAsync(
            ExecuteStepCommand command)
        {
            Task.Run(async () =>
            {
                try
                {
                    using (IServiceScope scope = serviceScopeFactory.CreateScope())
                    {
                        // when Saga is forced to run asynchronously, then it can be resumed
                        if (command?.Saga?.ExecutionState != null)
                            command.Saga.ExecutionState.CanBeResumed = true;

                        await DispatchStepSync(scope.ServiceProvider, command);
                    }
                }
                catch (Exception ex)
                {
                    await asyncErrorHandler.Handle(command.Saga, ex);
                }
            });
        }

        private async Task<ISaga> DispatchStepSync(
            IServiceProvider serviceProvider,
            ExecuteStepCommand command)
        {
            ExecuteStepCommandHandler stepExecutor = ActivatorUtilities.
                CreateInstance<ExecuteStepCommandHandler>(serviceProvider);

            ISaga saga = await stepExecutor.
                Handle(command);

            if (saga == null)
            {
                await messageBus.Publish(
                    new ExecutionEndMessage(command.Saga, command.Saga?.ExecutionState?.CurrentError));

                return null;
            }
            else
            {
                if (saga.IsIdle())
                {
                    await messageBus.Publish(
                        new ExecutionEndMessage(saga, saga.ExecutionState?.CurrentError));

                    if (saga.HasError())
                        throw saga.ExecutionState.CurrentError;

                    return saga;
                }
                else
                {
                    return await Handle(new ExecuteActionCommand()
                    {
                        Async = AsyncExecution.False(),
                        Saga = saga,
                        Model = command.Model
                    });
                }
            }
        }
    }
}
