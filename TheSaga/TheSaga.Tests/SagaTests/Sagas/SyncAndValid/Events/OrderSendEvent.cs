using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndValid.Events
{
    public class OrderSendEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}