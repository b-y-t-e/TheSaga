using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.SyncAndValid.Events
{
    public class DeliveredEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
