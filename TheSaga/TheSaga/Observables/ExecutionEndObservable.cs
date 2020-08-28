using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.MessageBus;
using TheSaga.Messages;

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
            var internalMessageBus = serviceProvider.GetRequiredService<IMessageBus>();

            internalMessageBus.Subscribe<ExecutionEndMessage>(this, OnSagaProcessingEnd);
        }

        public void Unsubscribe()
        {
            var internalMessageBus = serviceProvider.GetRequiredService<IMessageBus>();

            internalMessageBus.Unsubscribe<ExecutionEndMessage>(this);
        }

        private async Task OnSagaProcessingEnd(ExecutionEndMessage msg)
        {
        }
    }
}