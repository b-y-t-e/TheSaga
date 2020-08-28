using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheSaga.Exceptions;
using TheSaga.Execution.AsyncHandlers;
using TheSaga.InternalMessages;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Utils;

namespace TheSaga.Coordinators.AsyncHandlers
{
    internal class SagaExecutionEndObservable : IObservable
    {
        private IServiceProvider serviceProvider;

        public SagaExecutionEndObservable(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        async Task OnSagaProcessingEnd(SagaExecutionEndMessage msg)
        {
        }

        public void Subscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IInternalMessageBus>();

            internalMessageBus.
                Subscribe<SagaExecutionEndMessage>(this, OnSagaProcessingEnd);
        }

        public void Unsubscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IInternalMessageBus>();

            internalMessageBus.
                Unsubscribe<SagaExecutionEndMessage>(this);
        }
    }
}