using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.SyncAndValid.Events
{
    public class OrderCompletedEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
