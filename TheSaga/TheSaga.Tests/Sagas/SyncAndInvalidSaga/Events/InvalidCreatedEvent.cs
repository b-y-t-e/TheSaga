using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndInvalidSaga.Events
{
    public class InvalidCreatedEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}