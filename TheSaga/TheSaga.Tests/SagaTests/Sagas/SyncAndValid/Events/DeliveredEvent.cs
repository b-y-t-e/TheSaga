using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndValid.Events
{
    public class DeliveredEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}