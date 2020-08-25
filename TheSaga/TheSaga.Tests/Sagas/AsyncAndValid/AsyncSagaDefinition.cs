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
                Start<CreatedEvent, CreatedEventHandler>().
                ThenAsync(ctx =>
                {
                    ctx.State.Logs.Add("1");
                    return Task.Delay(TimeSpan.FromSeconds(1));
                }).
                Then(ctx =>
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