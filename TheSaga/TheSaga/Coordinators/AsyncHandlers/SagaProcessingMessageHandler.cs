using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.InternalMessages;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;
using TheSaga.States;
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