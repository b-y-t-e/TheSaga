using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndValid.Events
{
    public class OrderCompletedEvent : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}