using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Models;
using TheSaga.Tests.Sagas.AsyncSaga.EventHandlers;
using TheSaga.Tests.Sagas.AsyncSaga.Events;

namespace TheSaga.Tests.Sagas.AsyncSaga
{
    public class AsyncSagaDefinition : ISagaModelDefintion<AsyncState>
    {
        public ISagaModel<AsyncState> GetModel(IServiceProvider serviceProvider)
        {
            ISagaBuilder<AsyncState> builder = new SagaBuilder<AsyncState>(serviceProvider);

            builder.
                Start<CreatedEvent, CreatedEventHandler>().
                ThenAsync(ctx =>
                {
                    ctx.State.Logs.Add("1");
                    return Task.Delay(TimeSpan.FromSeconds(1));
                }).
                ThenAsync(ctx =>
                {
                    ctx.State.Logs.Add("2");
                    return Task.Delay(TimeSpan.FromSeconds(1));
                }).
                Finish();

            return builder.
                Build();
        }
    }
}