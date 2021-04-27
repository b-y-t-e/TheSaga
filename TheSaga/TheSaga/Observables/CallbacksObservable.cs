using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Exceptions;
using TheSaga.Locking;
using TheSaga.MessageBus;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Messages;
using TheSaga.Observables.Interfaces;

namespace TheSaga.Observables
{
    internal class CallbacksObservable : IObservable
    {
        private readonly IServiceProvider serviceProvider;

        public CallbacksObservable(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Subscribe()
        {
            IMessageBus messageBus = serviceProvider.GetRequiredService<IMessageBus>();

            messageBus.Subscribe<ExecutionStartMessage>(this, OnSagaProcessingStart);
            messageBus.Subscribe<ExecutionEndMessage>(this, OnSagaProcessingEnd);
        }

        public void Unsubscribe()
        {
            IMessageBus messageBus = serviceProvider.GetRequiredService<IMessageBus>();

            messageBus.Unsubscribe<ExecutionStartMessage>(this);
            messageBus.Unsubscribe<ExecutionEndMessage>(this);
        }


        private async Task OnSagaProcessingStart(ExecutionStartMessage msg)
        {
            await Callbacks.ExecuteBeforeRequestCallbacks(serviceProvider, msg.Saga);
        }

        private async Task OnSagaProcessingEnd(ExecutionEndMessage msg)
        {
            await Callbacks.ExecuteAfterRequestCallbacks(serviceProvider, msg.Saga, msg.Error);
        }
    }
}
