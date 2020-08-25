using System;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Coordinators;
using TheSaga.Tests.Sagas.OrderTestSaga.Events;

namespace TheSaga.Tests.Sagas.OrderTestSaga.EventHandlers
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