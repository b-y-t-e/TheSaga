using System;
using TheSaga.Builders;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.Tests.SagaTests.SendTests.Events;
using TheSaga.Tests.SagaTests.SendTests.States;

namespace TheSaga.Tests.SagaTests.SendTests
{
    public class SendTestsBuilder : ISagaModelBuilder<SendTestsData>
    {

        ISagaBuilder<SendTestsData> builder;

        public SendTestsBuilder(ISagaBuilder<SendTestsData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel Build()
        {
            builder.
                Name(nameof(SendTestsBuilder));

            builder.
                Start<SendCreateEvent>().
                TransitionTo<Init>();

            builder.
                Start<SendAlternativeCreateEvent>().
                TransitionTo<AkternativeInit>();

            builder.
                During<Init>().
                When<TestSendActionEvent>().
                Publish<SendAlternativeCreateEvent>((ctx, ev) =>
                    ctx.Data.CreatedSagaID = ev.ID = Guid.NewGuid()).
                TransitionTo<AfterInit>();

            return builder.
                Build();
        }
    }
}
