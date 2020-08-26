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

namespace TheSaga.Execution.AsyncHandlers
{
    internal class SagaAsyncStepCompletedHandler<TSagaState>
        where TSagaState : ISagaState
    {
        private ISagaExecutor sagaExecutor;
        private ISagaPersistance sagaPersistance;
        private IInternalMessageBus internalMessageBus;

        public SagaAsyncStepCompletedHandler(ISagaExecutor sagaExecutor, ISagaPersistance sagaPersistance, IInternalMessageBus internalMessageBus)
        {
            this.sagaExecutor = sagaExecutor;
            this.sagaPersistance = sagaPersistance;
            this.internalMessageBus = internalMessageBus;
        }

        public void Subscribe()
        {
            this.internalMessageBus.
                Subscribe<SagaAsyncStepCompletedMessage>(this, HandleAsyncStepCompletedMessage);
        }

        public void Unsubscribe()
        {
            this.internalMessageBus.
                Unsubscribe<SagaAsyncStepCompletedMessage>(this);
        }

        private Task HandleAsyncStepCompletedMessage(SagaAsyncStepCompletedMessage message)
        {
            if (message.SagaStateType != typeof(TSagaState))
                return Task.CompletedTask;

            return sagaExecutor.
                Handle(message.CorrelationID, null, true);
        }

    }
}