using System.Threading.Tasks;
using TheSaga.Exceptions;
using TheSaga.InternalMessages;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Utils;

namespace TheSaga.Coordinators.AsyncHandlers
{
    internal class SagaProcessingMessageHandler
    {
        private IInternalMessageBus internalMessageBus;

        public SagaProcessingMessageHandler(IInternalMessageBus internalMessageBus)
        {
            this.internalMessageBus = internalMessageBus;
        }

        public void Subscribe()
        {
            this.internalMessageBus.Subscribe<SagaProcessingStartMessage>(this, msg =>
            {
                if (!CorrelationIdLock.Acquire(msg.CorrelationID))
                    throw new SagaIsBusyException(msg.CorrelationID);
                return Task.CompletedTask;
            });
            this.internalMessageBus.Subscribe<SagaProcessingCompletedMessage>(this, msg =>
            {
                CorrelationIdLock.Banish(msg.CorrelationID);
                return Task.CompletedTask;
            });
        }

        public void Unsubscribe()
        {
            this.internalMessageBus.Unsubscribe<SagaProcessingStartMessage>(this);
            this.internalMessageBus.Unsubscribe<SagaProcessingCompletedMessage>(this);
        }
    }
}