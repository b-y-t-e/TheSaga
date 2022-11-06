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
                    OnResumeDoCurrentStepCompensation()).

                Start<CreateEvent>().
                    TransitionTo<InitState>().

                Start<CreateNewSaga>().
                    HandleBy<CreateNewSagaHandler1>().
                    HandleBy<CreateNewSagaHandler2>().
                    HandleBy<CreateNewSagaHandler3>().
                    Then(async ctx => { }).
                    /*Then(async ctx => { }).
                    Then(async ctx => { }).
                    Then(async ctx =>
                    {
                        if (ResumeSagaSettings.StopSagaExecution) await ctx.Stop();
                        if (ResumeSagaSettings.ThrowError) throw new System.Exception("!!");
                    }).*/
                    TransitionTo<SecondState>().

                Start<CreateWithBreakEvent>().
                    Then(async ctx => { if (ResumeSagaSettings.StopSagaExecution) await ctx.Stop(); }).
                    TransitionTo<InitState>().

                Start<CreateWithErrorEvent>().
                    Then(async ctx => { if (ResumeSagaSettings.StopSagaExecution) await ctx.Stop(); }).
                    Then(async ctx => throw new System.Exception("!!!")).

                During<InitState>().
                    When<ResumeSagaUpdateEvent>().
                        Then(async ctx => { if (ResumeSagaSettings.StopSagaExecution) await ctx.Stop(); }).
                        TransitionTo<SecondState>().
                    When<CreateNewSagaEvent>().
                        Publish<CreateNewSaga>((data, @event) =>
                        {
                            @event.ID = (data.ExecutionState.CurrentEvent as CreateNewSagaEvent).NewID;
                        }).
                        Then(async ctx =>
                        {
                            if (ResumeSagaSettings.StopSagaExecution) await ctx.Stop();
                        }).
                        TransitionTo<SecondState>();

            return builder.
                Build();
        }
    }
}
