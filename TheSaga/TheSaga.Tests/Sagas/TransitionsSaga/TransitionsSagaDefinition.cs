using System;
using TheSaga.Builders;
using TheSaga.Models;
using TheSaga.Tests.Sagas.TransitionsSaga.Events;
using TheSaga.Tests.Sagas.TransitionsSaga.States;

namespace TheSaga.Tests.Sagas.TransitionsSaga
{
    public class TransitionsSagaDefinition : ISagaModelDefintion<TransitionsSagaData>
    {
        public ISagaModel<TransitionsSagaData> GetModel(IServiceProvider serviceProvider)
        {
            ISagaBuilder<TransitionsSagaData> builder = new SagaBuilder<TransitionsSagaData>(serviceProvider);

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