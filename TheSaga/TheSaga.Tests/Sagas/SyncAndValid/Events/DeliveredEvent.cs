using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndValid.Events
{
    public class DeliveredEvent : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}