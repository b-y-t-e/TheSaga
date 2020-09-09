using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Builders.Handlers;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.ModelsHandlers;
using TheSaga.SagaModels;

namespace TheSaga.Tests.HanderTests
{
    public class SampleHandlerBuilder
    {
        private readonly IHandlersBuilder builder;

        public SampleHandlerBuilder(IHandlersBuilder builder)
        {
            this.builder = builder;
        }

        public IHandlersModel Build()
        {
            builder.

                When<OrderCreated>().
                    HandleBy<OrderCreated1Handler>().
                    HandleBy<OrderCreated2Handler>().
                
                When<OrderCreated>().
                    HandleBy<OrderCreated1Handler>().
                    HandleBy<OrderCreated2Handler>();

            return builder.
                Build();
        }
    }

    internal class OrderCreated1Handler : IHandlersCompensateEventHandler<OrderCreated>
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

    internal class OrderCreated : IHandlersEvent
    {
        public Guid ID => throw new NotImplementedException();
    }
}