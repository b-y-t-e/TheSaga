using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.SagaModels;
using TheSaga.Tests.Sagas.ResumeSaga.Events;
using TheSaga.Tests.Sagas.ResumeSaga.States;

namespace TheSaga.Tests.Sagas.ResumeSaga
{
    public class ResumeSagaBuilder : ISagaModelBuilder<ResumeSagaData>
    {

        ISagaBuilder<ResumeSagaData> builder;

        public ResumeSagaBuilder(ISagaBuilder<ResumeSagaData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel<ResumeSagaData> Build()
        {
            builder.
                Name(nameof(ResumeSagaBuilder));

            builder.
                Start<ResumeSagaCreateEvent>().
                TransitionTo<Init>();

            builder.
                During<Init>().
                When<ResumeSagaUpdateEvent>().
                Then(async ctx => { if (ResumeSagaSettings.StopSagaExecution) await ctx.Stop(); }).
                TransitionTo<SecondState>();

            return builder.
                Build();
        }
    }
}