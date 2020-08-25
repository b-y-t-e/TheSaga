using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Models;
using TheSaga.Tests.Sagas.AsyncAndValid.EventHandlers;
using TheSaga.Tests.Sagas.AsyncAndValid.Events;

namespace TheSaga.Tests.Sagas.AsyncAndValid
{
    public class AsyncSagaDefinition : ISagaModelDefintion<AsyncState>
    {
        public ISagaModel<AsyncState> GetModel(IServiceProvider serviceProvider)
        {
            ISagaBuilder<AsyncState> builder = new SagaBuilder<AsyncState>(serviceProvider);

            builder.
                Start<CreatedEvent, CreatedEventHandler>("CreatedEventStep0").
                ThenAsync("CreatedEventStep1", ctx =>
                {
                    return Task.Delay(TimeSpan.FromSeconds(1));
                }).
                Then("CreatedEventStep2", ctx =>
                {
                    return Task.Delay(TimeSpan.FromSeconds(1));
                }).
                Finish();

            return builder.
                Build();
        }
    }
}