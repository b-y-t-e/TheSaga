using System;
using TheSaga.Builders;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.Tests.SagaTests.MoveToSaga.Events;
using TheSaga.Tests.SagaTests.MoveToSaga.States;
using TheSaga.Tests.SagaTests.MoveToSaga;

namespace TheSaga.Tests.SagaTests.MoveToSaga
{
    public class MoveToSagaBuilder : ISagaModelBuilder<TransitionsSagaData>
    {
        ISagaBuilder<TransitionsSagaData> builder;

        public MoveToSagaBuilder(ISagaBuilder<TransitionsSagaData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel Build()
        {
            builder.
                Name(nameof(MoveToSagaBuilder));

            builder.
                Start<CreateMoveToSaga>().
                MoveTo<Init, InvalidUpdateEvent>();

            builder.
                During<Init>().
                When<InvalidUpdateEvent>().
                TransitionTo<SecondState>().
                Finish();

            return builder.
                Build();
        }
    }
}
