using TheSaga.Builders;
using TheSaga.SagaModels;
using TheSaga.Tests.SagaTests.Sagas.IfElseSaga.Events;
using TheSaga.Tests.SagaTests.Sagas.IfElseSaga.States;

namespace TheSaga.Tests.SagaTests.Sagas.IfElseSaga
{
    public class IfElseSagaBuilder : ISagaModelBuilder<IfElseSagaData>
    {

        ISagaBuilder<IfElseSagaData> builder;

        public IfElseSagaBuilder(ISagaBuilder<IfElseSagaData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel Build()
        {
            builder.
                Name(nameof(IfElseSagaBuilder)).

                Start<CreateIfElseSagaEvent>().
                    TransitionTo<Init>().

                During<Init>().
                    When<Test1Event>().
                        HandleBy<Test1EventHandler>().
                    If(async c => c.Data.Condition == 1, b => b.
                        Then(async c => c.Data.Value1 = new TrueValue())).
                    TransitionTo<SecondState>().

                During<Init>().
                    When<Test2Event>().
                        HandleBy<Test2EventHandler>().
                    If(async c => c.Data.Condition == 1, b => b.
                        Then(async c => c.Data.Value1 = new TrueValue())).
                    TransitionTo<SecondState>().

                During<Init>().
                    When<Test3Event>().
                        HandleBy<Test3EventHandler>().
                    If(async c => c.Data.Condition == 1, b => b.
                        Then(async c => c.Data.Value1 = new TrueValue())).
                    Else(b=> b.
                        Then(async c => c.Data.Value1 = new FalseValue())).
                    TransitionTo<SecondState>().

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