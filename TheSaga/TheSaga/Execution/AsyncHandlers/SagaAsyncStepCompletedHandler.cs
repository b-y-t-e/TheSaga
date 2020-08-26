using System.Threading.Tasks;
using TheSaga.Execution.Actions;
using TheSaga.InternalMessages;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Persistance;
using TheSaga.SagaStates;

namespace TheSaga.Execution.AsyncHandlers
{
    internal class SagaAsyncStepCompletedHandler<TSagaState>
        where TSagaState : ISagaState
    {
        private IInternalMessageBus internalMessageBus;
        private ISagaExecutor sagaExecutor;

        public SagaAsyncStepCompletedHandler(
            ISagaExecutor sagaExecutor, 
            IInternalMessageBus internalMessageBus)
        {
            this.sagaExecutor = sagaExecutor;
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
                Handle(message.CorrelationID, null, IsExecutionAsync.True());
        }
    }
}