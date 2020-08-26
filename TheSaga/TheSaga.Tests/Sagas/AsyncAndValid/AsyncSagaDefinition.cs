using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Models;
using TheSaga.Tests.Sagas.AsyncAndValid.EventHandlers;
using TheSaga.Tests.Sagas.AsyncAndValid.Events;

namespace TheSaga.Tests.Sagas.AsyncAndValid
{
    public class AsyncSagaDefinition : ISagaModelDefintion<AsyncData>
    {
        public ISagaModel<AsyncData> GetModel(IServiceProvider serviceProvider)
        {
            ISagaBuilder<AsyncData> builder = new SagaBuilder<AsyncData>(serviceProvider);

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