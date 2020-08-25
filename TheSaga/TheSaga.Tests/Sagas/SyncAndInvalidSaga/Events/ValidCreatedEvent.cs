using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndInvalidSaga.Events
{
    public class ValidCreatedEvent : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}