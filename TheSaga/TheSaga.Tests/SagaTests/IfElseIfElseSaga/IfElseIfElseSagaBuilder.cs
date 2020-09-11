using TheSaga.Builders;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.Tests.SagaTests.IfElseIfElseSaga.Classes;
using TheSaga.Tests.SagaTests.IfElseIfElseSaga.Events;
using TheSaga.Tests.SagaTests.IfElseIfElseSaga.States;

namespace TheSaga.Tests.SagaTests.IfElseIfElseSaga
{
    public class IfElseIfElseSagaBuilder : ISagaModelBuilder<IfElseIfElseSagaData>
    {

        ISagaBuilder<IfElseIfElseSagaData> builder;

        public IfElseIfElseSagaBuilder(ISagaBuilder<IfElseIfElseSagaData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel Build()
        {
            builder.
                Name(nameof(IfElseIfElseSagaBuilder)).

                Start<CreateIfElseSagaEvent>().
                    TransitionTo<Init>().

                During<Init>().
                    When<Test4Event>().
                        HandleBy<Test4EventHandler>().
                    If(async c => c.Data.Condition == 1, b => b.
                        Then(async c => c.Data.Value1 = 1)).
                    ElseIf(async c => c.Data.Condition == 2, b => b.
                        Then(async c => c.Data.Value1 = 2)).
                    Else(b => b.
                        Then(async c => c.Data.Value1 = 3)).
                    TransitionTo<SecondState>(); 

            return builder.
                Build();
        }
    }
}
