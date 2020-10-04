# TheSaga
 .net core Saga / ProcessManager implementation

```c#
    public class OrderSagaBuilder : ISagaModelBuilder<OrderData>
    {
        ISagaBuilder<OrderData> builder;

        public OrderSagaBuilder(ISagaBuilder<OrderData> builder) =>        
            this.builder = builder;       

        public ISagaModel Build()
        {
            builder.
                Name("Order").
                
                Start<OrderCreatedEvent>().
                    HandleBy<OrderCreatedEventHandler>().
                    Then<SendEmailOrderCreated>().
                    TransitionTo<Created>().
                    
                During<Created>().
                    When<OrderCompletedEvent>().
                        Then<SendEmailOrderCompleted>().
                        Then<OrderCourier>().
                        TransitionTo<Completed>().
                        
                During<Completed>().
                    When<OrderSendEvent>().
                        Then<SendEmailOrderCompleted>().
                        TransitionTo<OrderSend>().
                        
                During<OrderSend>().
                    When<DeliveredEvent>().
                        Then<SendEmailOrderDelivered>().
                        Then<NotifyErp>().
                        Finish();
                        
            return builder.Build();
        }
    }
```
