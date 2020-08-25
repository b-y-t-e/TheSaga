using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndInvalidSaga.Events
{
    public class InvalidUpdateEvent : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}