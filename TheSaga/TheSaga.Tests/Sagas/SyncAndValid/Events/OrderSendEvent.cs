using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndValid.Events
{
    public class OrderSendEvent : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}