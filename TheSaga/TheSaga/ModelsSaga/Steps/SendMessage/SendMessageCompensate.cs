using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;

namespace TheSaga.SagaModels.Steps.SendMessage
{
    internal class SendMessageCompensate<TSagaData, TEvent> : ISagaEventHandler<TSagaData, TEvent>
        where TSagaData : ISagaData
        where TEvent : ISagaEvent
    {
        private readonly ISagaCoordinator sagaCoordinator;

        public SendMessageCompensate(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
        }

        public async Task Execute(IExecutionContext<TSagaData> context, TEvent @event)
        {
        }

        public async Task Compensate(IExecutionContext<TSagaData> context, TEvent @event)
        {
            await sagaCoordinator.Publish(@event);
        }
    }
}