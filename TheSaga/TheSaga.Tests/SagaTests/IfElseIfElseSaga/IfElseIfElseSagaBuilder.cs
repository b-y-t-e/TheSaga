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
                        If(c => c.Data.Condition == 1, b => b.
                            Then(c => c.Data.Value1 = 1)).
                        ElseIf(c => c.Data.Condition == 2, b => b.
                            Then(c => c.Data.Value1 = 2)).
                        ElseIf(c => c.Data.Condition == 3, b => b.
                            If(c => c.Data.SubCondition == 0, b => b.
                                Then(c => c.Data.Value1 = 3)).
                            Else(b => b.
                                Then(c => c.Data.Value1 = 33))).
                        ElseIf(c => c.Data.Condition == 4, b => b.
                            If(c => c.Data.SubCondition == 1, b => b.
                                Then(c => c.Data.Value1 = 4))).
                        ElseIf(c => c.Data.Condition == 5, b => b.
                            Then(c => c.Data.Value1 = 5)).
                        Else(b => b.
                            Then(c => c.Data.Value1 = 100));

            return builder.
                Build();
        }
    }
}
