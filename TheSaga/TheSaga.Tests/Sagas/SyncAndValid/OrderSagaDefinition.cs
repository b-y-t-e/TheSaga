using System;
using TheSaga.Builders;
using TheSaga.Models;
using TheSaga.Tests.Sagas.SyncAndValid.Activities;
using TheSaga.Tests.Sagas.SyncAndValid.EventHandlers;
using TheSaga.Tests.Sagas.SyncAndValid.Events;
using TheSaga.Tests.Sagas.SyncAndValid.States;

namespace TheSaga.Tests.Sagas.SyncAndValid
{
    public class OrderSagaDefinition : ISagaModelDefintion<OrderState>
    {
        public ISagaModel<OrderState> GetModel(IServiceProvider serviceProvider)
        {
            ISagaBuilder<OrderState> builder = new SagaBuilder<OrderState>(serviceProvider);

            builder.
                Start<OrderCreatedEvent, OrderCreatedEventHandler>().
                Then(async ctx => { ctx.State.Logs.Add(nameof(OrderCreatedEvent)); }).
                TransitionTo<StateCreated>();

            builder.
                During<StateCreated>().
                When<OrderCompletedEvent>().
                Then(async ctx => { ctx.State.Logs.Add(nameof(OrderCompletedEvent)); }).
                Then<SendEmailToClientEvent>("email").
                Then<SendMessageToTheManagerEvent>().
                Then<OrderCourierEvent>().
                TransitionTo<StateCompleted>();

            builder.
                During<StateCompleted>().
                When<OrderSendEvent>().
                Then(async ctx => { ctx.State.Logs.Add(nameof(OrderSendEvent)); }).
                TransitionTo<StateOrderSend>();

            builder.
                During<StateOrderSend>().
                When<DeliveredEvent>().
                Then(async ctx => { ctx.State.Logs.Add(nameof(DeliveredEvent)); }).
                Finish();

            return builder.
                Build();
        }
    }
}