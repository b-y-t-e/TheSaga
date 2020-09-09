using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.SagaModels;
using TheSaga.Tests.SagaTests.Sagas.AsyncAndValid.EventHandlers;
using TheSaga.Tests.SagaTests.Sagas.AsyncAndValid.Events;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncAndValid
{
    public class AsyncSagaBuilder : ISagaModelBuilder<AsyncData>
    {
        ISagaBuilder<AsyncData> builder;

        public AsyncSagaBuilder(ISagaBuilder<AsyncData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel Build()
        {
            builder.
                Start<CreatedEvent, CreatedEventHandler>(
                    "CreatedEventStep0").
                ThenAsync(
                    "CreatedEventStep1",
                    ctx => Task.Delay(TimeSpan.FromSeconds(1))).
                Then(
                    "CreatedEventStep2",
                    ctx => Task.Delay(TimeSpan.FromSeconds(1))).
                Finish();

            return builder.
                Build();
        }
    }
}