using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga.Events
{
    public class InvalidCreatedEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}