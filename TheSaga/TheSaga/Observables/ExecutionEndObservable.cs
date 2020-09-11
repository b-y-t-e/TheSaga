using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.MessageBus;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Messages;
using TheSaga.Observables.Interfaces;

namespace TheSaga.Observables
{
    internal class ExecutionEndObservable : IObservable
    {
        private readonly IServiceProvider serviceProvider;

        public ExecutionEndObservable(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Subscribe()
        {
            IMessageBus messageBus = serviceProvider.GetRequiredService<IMessageBus>();

            messageBus.Subscribe<ExecutionEndMessage>(this, OnSagaProcessingEnd);
        }

        public void Unsubscribe()
        {
            IMessageBus messageBus = serviceProvider.GetRequiredService<IMessageBus>();

            messageBus.Unsubscribe<ExecutionEndMessage>(this);
        }

        private async Task OnSagaProcessingEnd(ExecutionEndMessage msg)
        {
        }
    }
}
