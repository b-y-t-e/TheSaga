﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Models;

namespace TheSaga.Commands.Handlers
{
    internal class ExecuteSagaCommandHandler
    {
        private IServiceProvider serviceProvider;

        public ExecuteSagaCommandHandler(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<ISaga> Handle(ExecuteSagaCommand command)
        {
            if (command.Event == null)
                command.Event = new EmptyEvent();

            ExecuteActionCommandHandler executeActionHandler = ActivatorUtilities.
               CreateInstance<ExecuteActionCommandHandler>(serviceProvider);

            ExecuteActionResult stepExecutionResult = await executeActionHandler.Handle(
                new ExecuteActionCommand()
                {
                    ID = command.ID,
                    Async = command.Async,
                    Event = command.Event,
                    Model = command.Model
                });

            if (stepExecutionResult.IsSyncProcessingComplete)
                return stepExecutionResult.Saga;

            return await Handle(new ExecuteSagaCommand()
            {
                Model = command.Model,
                ID = command.ID,
                Event = new EmptyEvent(),
                Async = command.Async
            });
        }
    }
}