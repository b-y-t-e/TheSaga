using TheSaga.Builders;
using TheSaga.SagaModels;
using TheSaga.Tests.SagaTests.Sagas.ChildStepsSaga.Activities;
using TheSaga.Tests.SagaTests.Sagas.ChildStepsSaga.Events;
using TheSaga.Tests.SagaTests.Sagas.ChildStepsSaga.States;

namespace TheSaga.Tests.SagaTests.Sagas.ChildStepsSaga
{
    public class ChildStepsSagaBuilder : ISagaModelBuilder<ChildStepsSagaData>
    {

        ISagaBuilder<ChildStepsSagaData> builder;

        public ChildStepsSagaBuilder(ISagaBuilder<ChildStepsSagaData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel Build()
        {
            builder.
                Name(nameof(ChildStepsSagaBuilder));

            builder.
                Start<ChildStepsSagaCreateEvent>().
                Do(b => b.
                    Then<InnerActivity1>().
                    Do(b => b.
                        Then<InnerActivity2>()).
                    Then<InnerActivity3>()).
                TransitionTo<Init>();

            return builder.
                Build();
        }
    }
}