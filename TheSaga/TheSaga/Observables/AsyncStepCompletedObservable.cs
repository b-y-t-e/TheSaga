using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheSaga.Commands;
using TheSaga.Commands.Handlers;
using TheSaga.Events;
using TheSaga.MessageBus;
using TheSaga.Messages;
using TheSaga.ValueObjects;

namespace TheSaga.Observables
{
    internal class AsyncStepCompletedObservable : IObservable
    {
        private IServiceProvider serviceProvider;

        public AsyncStepCompletedObservable(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Subscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IMessageBus>();

            internalMessageBus.
                Subscribe<AsyncStepCompletedMessage>(this, HandleAsyncStepCompletedMessage);
        }

        public void Unsubscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IMessageBus>();

            internalMessageBus.
                Unsubscribe<AsyncStepCompletedMessage>(this);
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
    }
}