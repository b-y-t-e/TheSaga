using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndValid.Events
{
    public class OrderSendEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}