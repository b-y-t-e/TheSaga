using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Exceptions;
using TheSaga.Locking;
using TheSaga.Messages;
using TheSaga.Messages.MessageBus;

namespace TheSaga.Coordinators.Observables
{
    internal class LockingObservable : IObservable
    {
        private IServiceProvider serviceProvider;

        public LockingObservable(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }


        async Task OnSagaProcessingStart(ExecutionStartMessage msg)
        {
            var sagaLocking = serviceProvider.
                GetRequiredService<ISagaLocking>();

            if (!await sagaLocking.Acquire(msg.Saga.Data.ID))
                throw new SagaIsBusyException(msg.Saga.Data.ID);
        }

        async Task OnSagaProcessingEnd(ExecutionEndMessage msg)
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
                Subscribe<ExecutionStartMessage>(this, OnSagaProcessingStart);

            internalMessageBus.
                Subscribe<ExecutionEndMessage>(this, OnSagaProcessingEnd);
        }

        public void Unsubscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IInternalMessageBus>();

            internalMessageBus.
                Unsubscribe<ExecutionStartMessage>(this);
            
            internalMessageBus.
                Unsubscribe<ExecutionEndMessage>(this);
        }
    }
}