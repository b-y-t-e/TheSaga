using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Handlers.Events;
using TheSaga.Handlers.ExecutionContext;

namespace TheSaga.Tests.HanderTests
{
    internal class OrderCreated2Handler : IHandlersCompensateEventHandler<OrderCreated>
    {
        public Task Compensate(IHandlersExecutionContext context, OrderCreated @event)
        {
            throw new NotImplementedException();
        }

        public Task Execute(IHandlersExecutionContext context, OrderCreated @event)
        {
            throw new NotImplementedException();
        }
    }
}