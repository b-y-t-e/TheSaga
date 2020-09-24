using TheSaga.Builders;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.Tests.SagaTests.WhileSaga.Classes;
using TheSaga.Tests.SagaTests.WhileSaga.Events;
using TheSaga.Tests.SagaTests.WhileSaga.States;

namespace TheSaga.Tests.SagaTests.WhileSaga
{
    public class WhileSagaBuilder : ISagaModelBuilder<WhileSagaData>
    {

        ISagaBuilder<WhileSagaData> builder;

        public WhileSagaBuilder(ISagaBuilder<WhileSagaData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel Build()
        {
            builder.
                Name(nameof(WhileSagaBuilder)).

                Start<CreateWhileSagaEvent>().
                    TransitionTo<Init>().

                During<Init>().
                    When<Test1Event>().
                        HandleBy<Test1EventHandler>().
                        While(c => c.Data.Counter > 0, b => b.
                            Then(c => c.Data.Value += 10).
                            Then(c => c.Data.Counter--)).

                During<Init>().
                    When<Test2Event>().
                        HandleBy<Test2EventHandler>().
                        While(c => c.Data.Counter > 0, b => b.
                            Then(c => c.Data.Value += 10).
                            Then(c => c.Data.Counter--)).
                        Then(c => c.Data.SecondValue = 1).
                        Then(c => throw new System.Exception("!!!")).

                During<Init>().
                    When<Test3Event>().
                        Then(c => c.Data.Counter = 10).
                        Then(c => c.Data.Counter2 = 5).
                        While(c => c.Data.Counter > 0, b => b.
                            Then(c => c.Data.Value += 1).
                            Then(c => c.Data.Counter--)).
                        While(c => c.Data.Counter2 > 0, b => b.
                            Then(c => c.Data.Value += 1).
                            Then(c => c.Data.Counter2--));

            return builder.
                Build();
        }
    }
}
