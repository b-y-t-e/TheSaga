using System;
using System.Collections.Generic;
using System.Text;
using TheSaga.Builders;
using TheSaga.Interfaces;
using TheSaga.Models;

namespace TheSaga.Tests.Sagas.OrderTestSaga
{

    public class OrderSagaDefinition : ISagaModelDefintion<OrderState>
    {
        public SagaModel<OrderState> GetModel()
        {
            ISagaBuilder<OrderState> builder = new SagaBuilder<OrderState>();

            builder.
                Start<Utworzone>().
                Then(async ctx => { }).
                TransitionTo<Nowe>();

            builder.
                During<Nowe>().
                When<Skompletowano>().
                Then<WyslijEmailDoKlienta>("email").
                Then<WyslijWiadomoscDoKierownika>().
                Then<ZamowKuriera>().
                TransitionTo<Skompletowane>();

            builder.
                During<Skompletowane>().
                When<Wyslano>().
                Then(async ctx => { }).
                TransitionTo<Wyslane>();

            builder.
                During<Wyslane>().
                When<Dostarczono>().
                Then(async ctx => { }).
                TransitionTo<Zakonczono>();

            builder.
                During<Wyslane>().
                After(TimeSpan.FromDays(30)).
                Then(async ctx => { }).
                TransitionTo<Zakonczono>();

            return builder.
                Build();
        }
    }
}
