using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheSaga.Messages;
using TheSaga.Messages.MessageBus;

namespace TheSaga.Coordinators.Observables
{
    internal class ExecutionEndObservable : IObservable
    {
        private IServiceProvider serviceProvider;

        public ExecutionEndObservable(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Subscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IInternalMessageBus>();

            internalMessageBus.
                Subscribe<ExecutionEndMessage>(this, OnSagaProcessingEnd);
        }

        public void Unsubscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IInternalMessageBus>();

            internalMessageBus.
                Unsubscribe<ExecutionEndMessage>(this);
        }

        private async Task OnSagaProcessingEnd(ExecutionEndMessage msg)
        {
        }
    }
}