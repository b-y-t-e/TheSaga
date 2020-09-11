using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.SyncAndValid.Events
{
    public class OrderSendEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
