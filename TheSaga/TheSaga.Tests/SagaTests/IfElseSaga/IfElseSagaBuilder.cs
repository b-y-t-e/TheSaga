using TheSaga.Builders;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.Tests.SagaTests.IfElseSaga.Classes;
using TheSaga.Tests.SagaTests.IfElseSaga.Events;
using TheSaga.Tests.SagaTests.IfElseSaga.States;

namespace TheSaga.Tests.SagaTests.IfElseSaga
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
                    If(c => c.Data.Condition == 1, b => b.
                        Then(c => c.Data.Value1 = new TrueValue())).
                    // TransitionTo<SecondState>().

                During<Init>().
                    When<Test2Event>().
                        HandleBy<Test2EventHandler>().
                    If(c => c.Data.Condition == 1, b => b.
                        Then(c => c.Data.Value1 = new TrueValue())).
                    TransitionTo<SecondState>().

                During<Init>().
                    When<Test3Event>().
                        HandleBy<Test3EventHandler>().
                    If(c => c.Data.Condition == 1, b => b.
                        Then(c => c.Data.Value1 = new TrueValue())).
                    Else(b=> b.
                        Then(c => c.Data.Value1 = new FalseValue())).
                    TransitionTo<SecondState>();

            return builder.
                Build();
        }
    }
}
