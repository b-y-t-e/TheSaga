using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndValid.Events
{
    public class OrderCreatedEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}