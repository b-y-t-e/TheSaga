using System.Threading.Tasks;
using TheSaga.Exceptions;
using TheSaga.InternalMessages;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Utils;

namespace TheSaga.Coordinators.AsyncHandlers
{
    internal class SagaLockingHandler
    {
        private IInternalMessageBus internalMessageBus;
        private ISagaLocking sagaLocking;

        public SagaLockingHandler(IInternalMessageBus internalMessageBus, ISagaLocking sagaLocking)
        {
            this.internalMessageBus = internalMessageBus;
            this.sagaLocking = sagaLocking;
        }

        async Task OnSagaProcessingStart(SagaExecutionStartMessage msg)
        {
            if (!await sagaLocking.Acquire(msg.SagaID))
                throw new SagaIsBusyException(msg.SagaID);
        }

        async Task OnSagaProcessingEnd(SagaExecutionEndMessage msg)
        {
            await sagaLocking.Banish(msg.SagaID);
        }

        public void Subscribe()
        {
            this.internalMessageBus.
                Subscribe<SagaExecutionStartMessage>(this, OnSagaProcessingStart);

            this.internalMessageBus.
                Subscribe<SagaExecutionEndMessage>(this, OnSagaProcessingEnd);
        }

        public void Unsubscribe()
        {
            this.internalMessageBus.
                Unsubscribe<SagaExecutionStartMessage>(this);
            
            this.internalMessageBus.
                Unsubscribe<SagaExecutionEndMessage>(this);
        }
    }
}