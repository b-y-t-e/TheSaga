using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.SagaModels;
using TheSaga.Tests.SagaTests.Sagas.SyncAndValid.Activities;
using TheSaga.Tests.SagaTests.Sagas.SyncAndValid.EventHandlers;
using TheSaga.Tests.SagaTests.Sagas.SyncAndValid.Events;
using TheSaga.Tests.SagaTests.Sagas.SyncAndValid.States;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndValid
{
    public class OrderSagaBuilder : ISagaModelBuilder<OrderData>
    {
        ISagaBuilder<OrderData> builder;

        public OrderSagaBuilder(ISagaBuilder<OrderData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel Build()
        {
            builder.
                Name(nameof(OrderSagaBuilder));

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

            builder.
                During<StateCreated>().
                When<ToAlternative1Event>().                
                    TransitionTo<StateAlternative1>().
                When<ToAlternative2Event>().
                    TransitionTo<StateAlternative2>();

            return builder.
                Build();
        }
    }
}