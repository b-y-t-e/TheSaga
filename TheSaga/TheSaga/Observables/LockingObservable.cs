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
    internal class LockingObservable : IObservable
    {
        private readonly IServiceProvider serviceProvider;

        public LockingObservable(IServiceProvider serviceProvider)
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
            ISagaLocking sagaLocking = serviceProvider.GetRequiredService<ISagaLocking>();

            //if (msg.Saga?.Data?.ID != null)
            if (!await sagaLocking.Acquire(msg.SagaID))
                throw new SagaIsBusyException(msg.SagaID);
        }

        private async Task OnSagaProcessingEnd(ExecutionEndMessage msg)
        {
            ISagaLocking sagaLocking = serviceProvider.GetRequiredService<ISagaLocking>();

            if (msg.Saga?.Data?.ID != null)
                await sagaLocking.Banish(msg.Saga.Data.ID);
        }
    }
}
