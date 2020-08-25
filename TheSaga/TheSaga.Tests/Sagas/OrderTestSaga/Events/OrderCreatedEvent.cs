using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Events
{
    public class OrderCreatedEvent : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}