using TheSaga.Builders;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.Tests.SagaTests.ChildStepsSaga.Activities;
using TheSaga.Tests.SagaTests.ChildStepsSaga.Conditions;
using TheSaga.Tests.SagaTests.ChildStepsSaga.Events;
using TheSaga.Tests.SagaTests.ChildStepsSaga.States;

namespace TheSaga.Tests.SagaTests.ChildStepsSaga
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
                Start<SagaCreateWithDoStepEvent>().
                    Do(b => b.
                        Then<InnerActivity1>().
                        Do(b => b.
                            Then<InnerActivity2>()).
                        Then<InnerActivity3>()).
                    TransitionTo<Init>();

            builder.
                Start<SagaCreateWithIfStepEvent>().
                    Then<InnerActivity1>().
                    If<Condition1>(b => b.
                        Then<InnerActivity1>().
                        Then<InnerActivity3>()).
                    TransitionTo<Init>();

            return builder.
                Build();
        }
    }
}
