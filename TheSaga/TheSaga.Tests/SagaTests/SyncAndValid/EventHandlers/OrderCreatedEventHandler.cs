﻿using System.Threading;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Tests.SagaTests.SyncAndValid.Events;

namespace TheSaga.Tests.SagaTests.SyncAndValid.EventHandlers
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
    public class OrderCompletedEventHandler : ISagaEventHandler<OrderData, OrderCompletedEvent>
    {
        public OrderCompletedEventHandler()
        {
        }

        public Task Compensate(IExecutionContext<OrderData> context, OrderCompletedEvent @event)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext<OrderData> context, OrderCompletedEvent @event)
        {
            if(@event == null)
            {

            }
            Thread.Sleep(10000);
            return Task.CompletedTask;
        }
    }
}
