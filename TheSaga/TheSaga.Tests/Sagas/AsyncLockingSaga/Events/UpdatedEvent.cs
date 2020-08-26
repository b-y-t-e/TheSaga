using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.AsyncLockingSaga.Events
{
    public class UpdatedEvent : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}