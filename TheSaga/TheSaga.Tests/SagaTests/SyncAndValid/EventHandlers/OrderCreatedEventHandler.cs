using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Tests.SagaTests.Sagas.SyncAndValid.Events;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndValid.EventHandlers
{
    public class OrderCreatedEventHandler : ISagaEventHandler<OrderData, OrderCreatedEvent>
    {
        public OrderCreatedEventHandler()
        {
        }

        public Task Compensate(IExecutionContext<OrderData> context, OrderCreatedEvent @event)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext<OrderData> context, OrderCreatedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}