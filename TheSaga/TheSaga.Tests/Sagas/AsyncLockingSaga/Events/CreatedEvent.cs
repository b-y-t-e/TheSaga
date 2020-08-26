using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.AsyncLockingSaga.Events
{
    public class CreatedEvent : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}