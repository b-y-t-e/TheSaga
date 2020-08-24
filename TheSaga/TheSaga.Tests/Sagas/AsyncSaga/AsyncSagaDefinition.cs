using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Interfaces;
using TheSaga.Models;
using TheSaga.Tests.Sagas.AsyncSaga.EventHandlers;
using TheSaga.Tests.Sagas.AsyncSaga.Events;
using TheSaga.Tests.Sagas.AsyncSaga.States;

namespace TheSaga.Tests.Sagas.AsyncSaga
{

    public class AsyncSagaDefinition : ISagaModelDefintion<AsyncState>
    {
        public ISagaModel<AsyncState> GetModel(IServiceProvider serviceProvider)
        {
            ISagaBuilder<AsyncState> builder = new SagaBuilder<AsyncState>(serviceProvider);

            builder.
                Start<Utworzone, UtworzoneHandler>().
                Then(ctx =>
                {
                    ctx.State.Logs.Add("1");
                    return Task.Delay(TimeSpan.FromSeconds(3));
                }).
                Then(ctx =>
                {
                    ctx.State.Logs.Add("2");
                    return Task.Delay(TimeSpan.FromSeconds(3));
                }).
                TransitionTo<Nowe>();

            return builder.
                Build();
        }
    }
}
