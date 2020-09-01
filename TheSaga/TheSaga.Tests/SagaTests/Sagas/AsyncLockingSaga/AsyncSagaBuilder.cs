using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.SagaModels;
using TheSaga.Tests.SagaTests.Sagas.AsyncLockingSaga.EventHandlers;
using TheSaga.Tests.SagaTests.Sagas.AsyncLockingSaga.Events;
using TheSaga.Tests.SagaTests.Sagas.AsyncLockingSaga.States;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncLockingSaga
{
    public class AsyncSagaBuilder : ISagaModelBuilder<AsyncData>
    {
        ISagaBuilder<AsyncData> builder;

        public AsyncSagaBuilder(ISagaBuilder<AsyncData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel<AsyncData> Build()
        {
            builder.
                Start<CreatedEvent, CreatedEventHandler>().
                ThenAsync(ctx => Task.Delay(TimeSpan.FromSeconds(1))).
                TransitionTo<New>();

            builder.
                During<New>().
                When<UpdatedEvent>().
                    HandleBy<UpdatedEventHandler>().
                ThenAsync(ctx => Task.Delay(TimeSpan.FromSeconds(5))).
                Finish();

            return builder.
                Build();
        }
    }
}