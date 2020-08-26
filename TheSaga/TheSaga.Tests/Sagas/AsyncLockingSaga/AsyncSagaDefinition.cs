using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Models;
using TheSaga.Tests.Sagas.AsyncLockingSaga.EventHandlers;
using TheSaga.Tests.Sagas.AsyncLockingSaga.Events;
using TheSaga.Tests.Sagas.AsyncLockingSaga.States;

namespace TheSaga.Tests.Sagas.AsyncLockingSaga
{
    public class AsyncSagaDefinition : ISagaModelDefintion<AsyncState>
    {
        public ISagaModel<AsyncState> GetModel(IServiceProvider serviceProvider)
        {
            ISagaBuilder<AsyncState> builder = new SagaBuilder<AsyncState>(serviceProvider);

            builder.
                Start<CreatedEvent, CreatedEventHandler>().
                ThenAsync(ctx => Task.Delay(TimeSpan.FromSeconds(1))).
                TransitionTo<New>();

            builder.
                During<New>().
                When<UpdatedEvent>().
                ThenAsync(ctx => Task.Delay(TimeSpan.FromSeconds(2))).
                Finish();

            return builder.
                Build();
        }
    }
}