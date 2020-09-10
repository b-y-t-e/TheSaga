using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndValid.Events
{
    public class OrderCompletedEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}