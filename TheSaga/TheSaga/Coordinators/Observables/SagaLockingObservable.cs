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
    internal class SagaLockingObservable : IObservable
    {
        private IServiceProvider serviceProvider;

        public SagaLockingObservable(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }


        async Task OnSagaProcessingStart(SagaExecutionStartMessage msg)
        {
            var sagaLocking = serviceProvider.
                GetRequiredService<ISagaLocking>();

            if (!await sagaLocking.Acquire(msg.Saga.Data.ID))
                throw new SagaIsBusyException(msg.Saga.Data.ID);
        }

        async Task OnSagaProcessingEnd(SagaExecutionEndMessage msg)
        {
            var sagaLocking = serviceProvider.
                GetRequiredService<ISagaLocking>();

            await sagaLocking.Banish(msg.Saga.Data.ID);
        }

        public void Subscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IInternalMessageBus>();

            internalMessageBus.
                Subscribe<SagaExecutionStartMessage>(this, OnSagaProcessingStart);

            internalMessageBus.
                Subscribe<SagaExecutionEndMessage>(this, OnSagaProcessingEnd);
        }

        public void Unsubscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IInternalMessageBus>();

            internalMessageBus.
                Unsubscribe<SagaExecutionStartMessage>(this);
            
            internalMessageBus.
                Unsubscribe<SagaExecutionEndMessage>(this);
        }
    }
}