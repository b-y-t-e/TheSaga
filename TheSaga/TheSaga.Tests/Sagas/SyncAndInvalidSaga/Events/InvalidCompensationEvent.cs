using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndInvalidSaga.Events
{
    public class InvalidCompensationEvent : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}