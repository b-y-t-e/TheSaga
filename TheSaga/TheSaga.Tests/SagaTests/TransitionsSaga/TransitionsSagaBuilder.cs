using System;
using TheSaga.Builders;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.Tests.SagaTests.TransitionsSaga.Events;
using TheSaga.Tests.SagaTests.TransitionsSaga.States;

namespace TheSaga.Tests.SagaTests.TransitionsSaga
{
    public class TransitionsSagaBuilder : ISagaModelBuilder<TransitionsSagaData>
    {
        ISagaBuilder<TransitionsSagaData> builder;

        public TransitionsSagaBuilder(ISagaBuilder<TransitionsSagaData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel Build()
        {
            builder.
                Name(nameof(TransitionsSagaBuilder));

            builder.
                Start<CreateEvent>().
                TransitionTo<Init>();

            builder.
                During<Init>().
                When<InvalidUpdateEvent>().
                TransitionTo<SecondState>().
                Then(ctx => { throw new Exception(); }).
                    // RetryBy<InvalidUpdateEvent>().
                Finish();

            return builder.
                Build();
        }
    }
}
