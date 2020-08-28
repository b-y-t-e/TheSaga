using System;
using TheSaga.Builders;
using TheSaga.Models;
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
                Start<CreateEvent>().
                TransitionTo<Init>();

            builder.
                During<Init>().
                When<UpdateEvent>().
                TransitionTo<SecondState>().
                Then(ctx => { throw new Exception(); }).
                Finish();

            return builder.
                Build();
        }
    }
}