using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Execution.Context;
using TheSaga.Tests.Sagas.SyncAndValid.Events;

namespace TheSaga.Tests.Sagas.SyncAndValid.EventHandlers
{
    public class OrderCreatedEventHandler : IEventHandler<OrderState, OrderCreatedEvent>
    {
        public OrderCreatedEventHandler()
        {
        }

        public Task Compensate(IEventContext<OrderState, OrderCreatedEvent> context)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IEventContext<OrderState, OrderCreatedEvent> context)
        {
            return Task.CompletedTask;
        }
    }
}