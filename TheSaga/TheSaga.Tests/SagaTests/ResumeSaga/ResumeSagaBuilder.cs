using TheSaga.Builders;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.Tests.SagaTests.ResumeSaga.Events;
using TheSaga.Tests.SagaTests.ResumeSaga.States;

namespace TheSaga.Tests.SagaTests.ResumeSaga
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

                Start<CreateEvent>().
                    TransitionTo<Init>().

                Start<CreateWithBreakEvent>().
                    Then(async ctx => { if (ResumeSagaSettings.StopSagaExecution) await ctx.Stop(); }).
                    TransitionTo<Init>().

                Start<CreateWithErrorEvent>().
                    Then(async ctx => { if (ResumeSagaSettings.StopSagaExecution) await ctx.Stop(); }).
                    Then(async ctx => throw new System.Exception("!!!")).

                During<Init>().
                    When<ResumeSagaUpdateEvent>().
                    Then(async ctx => { if (ResumeSagaSettings.StopSagaExecution) await ctx.Stop(); }).
                    TransitionTo<SecondState>();

            return builder.
                Build();
        }
    }
}
