using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.SagaModels;
using TheSaga.Tests.Sagas.TransitionsSaga.Events;
using TheSaga.Tests.Sagas.TransitionsSaga.States;

namespace TheSaga.Tests.Sagas.TransitionsSaga
{
    public class TransitionsSagaBuilder : ISagaModelBuilder<TransitionsSagaData>
    {
        ISagaBuilder<TransitionsSagaData> builder;

        public TransitionsSagaBuilder(ISagaBuilder<TransitionsSagaData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel<TransitionsSagaData> Build()
        {
            builder.
                Name(nameof(TransitionsSagaBuilder));

            builder.
                Start<CreateEvent>().
                TransitionTo<Init>();

            builder.
                During<Init>().
                When<InvalidUpdateEvent>().
                TransitionTo<SecondState>().
                Then(ctx => { throw new Exception(); }).
                Finish();

            return builder.
                Build();
        }
    }
}