using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.AsyncAndValid.Events
{
    public class CreatedEvent : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}