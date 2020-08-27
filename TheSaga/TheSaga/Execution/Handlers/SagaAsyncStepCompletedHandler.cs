using System.Threading.Tasks;
using TheSaga.Execution.Actions;
using TheSaga.InternalMessages;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Persistance;
using TheSaga.SagaStates;

namespace TheSaga.Execution.AsyncHandlers
{
    internal class SagaAsyncStepCompletedHandler<TSagaData>
        where TSagaData : ISagaData
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
            if (message.SagaStateType != typeof(TSagaData))
                return Task.CompletedTask;

            return sagaExecutor.
                Handle(message.SagaID, null, IsExecutionAsync.True());
        }
    }
}