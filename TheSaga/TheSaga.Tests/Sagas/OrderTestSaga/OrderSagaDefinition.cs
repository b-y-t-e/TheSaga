using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TheSaga.Builders;
using TheSaga.Interfaces;
using TheSaga.Models;
using TheSaga.Tests.Sagas.OrderTestSaga.Activities;
using TheSaga.Tests.Sagas.OrderTestSaga.EventHandlers;
using TheSaga.Tests.Sagas.OrderTestSaga.Events;
using TheSaga.Tests.Sagas.OrderTestSaga.States;

namespace TheSaga.Tests.Sagas.OrderTestSaga
{

    public class OrderSagaDefinition : ISagaModelDefintion<OrderState>
    {
        public ISagaModel<OrderState> GetModel(IServiceProvider serviceProvider)
        {
            ISagaBuilder<OrderState> builder = new SagaBuilder<OrderState>(serviceProvider);

            builder.
                Start<Utworzone, UtworzoneHandler>().
                Then(async ctx => { ctx.State.Logs.Add(nameof(Utworzone)); }).
                TransitionTo<Nowe>();

            builder.
                During<Nowe>().
                When<Skompletowano>().
                Then(async ctx => { ctx.State.Logs.Add(nameof(Skompletowano)); }).
                Then<WyslijEmailDoKlienta>("email").
                Then<WyslijWiadomoscDoKierownika>().
                Then<ZamowKuriera>().
                TransitionTo<Skompletowane>();

            builder.
                During<Skompletowane>().
                When<Wyslano>().
                Then(async ctx => { ctx.State.Logs.Add(nameof(Wyslano)); }).
                TransitionTo<Wyslane>();

            builder.
                During<Wyslane>().
                When<Dostarczono>().
                Then(async ctx => { ctx.State.Logs.Add(nameof(Dostarczono)); }).
                TransitionTo<Zakonczono>();

            return builder.
                Build();
        }
    }
}
