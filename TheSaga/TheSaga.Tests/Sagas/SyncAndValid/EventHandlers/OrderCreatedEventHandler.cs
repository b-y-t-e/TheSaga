using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Execution.Context;
using TheSaga.Tests.Sagas.SyncAndValid.Events;

namespace TheSaga.Tests.Sagas.SyncAndValid.EventHandlers
{
    public class OrderCreatedEventHandler : IEventHandler<OrderState, OrderCreatedEvent>
    {
        private readonly ISagaCoordinator sagaCoordinator;

        public OrderCreatedEventHandler(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
        }

        public Task Compensate(IEventContext<OrderState, OrderCreatedEvent> context)
        {
            context.State.Logs.Add($"{nameof(OrderCreatedEventHandler)} compensation");
            return Task.CompletedTask;
        }

        public Task Execute(IEventContext<OrderState, OrderCreatedEvent> context)
        {
            context.State.Logs.Add(nameof(OrderCreatedEventHandler));
            return Task.CompletedTask;
        }
    }
}