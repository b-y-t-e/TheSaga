using Microsoft.Extensions.DependencyInjection;
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
            ISaga saga = command.Saga;
            if (saga == null)
                throw new SagaInstanceNotFoundException(command.Model.SagaStateType);

            ISagaStep step = command.Model.Actions.
                FindStep(saga, saga.ExecutionState.CurrentEvent.GetType());

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
                        //debug.Add( $"{saga.ExecutionState.ExecutionID} | {saga.Data.ID} | ended");
                        //Debug.WriteLine($"{saga.ExecutionState.ExecutionID} | {saga.Data.ID} ended");
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
                            Saga = saga,
                            Model = command.Model
                        });
                    }
                }
            }
        }
    }
}
