using TheSaga.Builders;
using TheSaga.Handlers.Builders;
using TheSaga.Handlers.ModelsHandlers;

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
}
