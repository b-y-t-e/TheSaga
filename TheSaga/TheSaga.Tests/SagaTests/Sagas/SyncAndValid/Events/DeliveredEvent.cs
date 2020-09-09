using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndValid.Events
{
    public class DeliveredEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}