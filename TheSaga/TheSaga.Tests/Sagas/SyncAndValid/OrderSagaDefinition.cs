using System;
using System.Threading.Tasks;
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
                Start<OrderCreatedEvent, OrderCreatedEventHandler>("OrderCreatedEventStep0").
                Then("OrderCreatedEventStep1", ctx => Task.CompletedTask).
                TransitionTo<StateCreated>();

            builder.
                During<StateCreated>().
                When<OrderCompletedEvent>().
                Then("OrderCompletedEventStep1", ctx => Task.CompletedTask).
                Then<SendEmailToClientEvent>("email").
                Then<SendMessageToTheManagerEvent>("SendMessageToTheManagerEventStep").
                Then<OrderCourierEvent>("OrderCourierEventStep").
                TransitionTo<StateCompleted>();

            builder.
                During<StateCompleted>().
                When<OrderSendEvent>().
                Then("OrderSendEventStep1", ctx => Task.CompletedTask).
                TransitionTo<StateOrderSend>();

            builder.
                During<StateOrderSend>().
                When<DeliveredEvent>().
                Then("DeliveredEventStep1", ctx => Task.CompletedTask).
                Finish();

            return builder.
                Build();
        }
    }
}