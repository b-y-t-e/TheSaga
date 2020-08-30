using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga.Events
{
    public class InvalidUpdateEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}