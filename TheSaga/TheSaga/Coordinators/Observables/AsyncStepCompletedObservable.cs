using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Events;
using TheSaga.Execution.Commands;
using TheSaga.Execution.Commands.Handlers;
using TheSaga.Messages;
using TheSaga.Messages.MessageBus;
using TheSaga.ValueObjects;

namespace TheSaga.Coordinators.Observables
{
    internal class AsyncStepCompletedObservable : IObservable
    {
        IServiceProvider serviceProvider;

        public AsyncStepCompletedObservable(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private Task HandleAsyncStepCompletedMessage(AsyncStepCompletedMessage message)
        {
            ExecuteSagaCommandHandler handler = serviceProvider.
                GetRequiredService<ExecuteSagaCommandHandler>();

            return handler.Handle(
                new ExecuteSagaCommand()
                {
                    Async = AsyncExecution.True(),
                    Event = new EmptyEvent(),
                    ID = message.SagaID,
                    Model = message.Model
                });
        }

        public void Subscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IInternalMessageBus>();

            internalMessageBus.
                Subscribe<AsyncStepCompletedMessage>(this, HandleAsyncStepCompletedMessage);
        }

        public void Unsubscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IInternalMessageBus>();

            internalMessageBus.
                Unsubscribe<AsyncStepCompletedMessage>(this);
        }

    }
}