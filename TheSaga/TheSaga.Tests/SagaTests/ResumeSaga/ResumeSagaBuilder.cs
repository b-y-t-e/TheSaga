using TheSaga.Builders;
using TheSaga.SagaModels;
using TheSaga.Tests.SagaTests.Sagas.ResumeSaga.Events;
using TheSaga.Tests.SagaTests.Sagas.ResumeSaga.States;

namespace TheSaga.Tests.SagaTests.Sagas.ResumeSaga
{
    public class ResumeSagaBuilder : ISagaModelBuilder<ResumeSagaData>
    {

        ISagaBuilder<ResumeSagaData> builder;

        public ResumeSagaBuilder(ISagaBuilder<ResumeSagaData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel Build()
        {
            builder.
                Name(nameof(ResumeSagaBuilder)).

                Settings(b => b.
                    OnResumeDoCurrentStepCompensation().
                    InHistoryStoreOnlyCurrentStep()).

                Start<ResumeSagaCreateEvent>().
                    TransitionTo<Init>().

                During<Init>().
                    When<ResumeSagaUpdateEvent>().
                    Then(async ctx => { if (ResumeSagaSettings.StopSagaExecution) await ctx.Stop(); }).
                    TransitionTo<SecondState>();

            return builder.
                Build();
        }
    }
}