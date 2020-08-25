using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Events
{
    public class DeliveredEvent : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}