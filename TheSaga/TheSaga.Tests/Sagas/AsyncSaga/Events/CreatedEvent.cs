using System;
using TheSaga.Interfaces;

namespace TheSaga.Tests.Sagas.AsyncSaga.Events
{
    public class CreatedEvent : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}
