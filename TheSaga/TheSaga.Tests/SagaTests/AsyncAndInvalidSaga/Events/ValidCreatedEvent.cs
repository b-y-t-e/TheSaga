using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.AsyncAndInvalidSaga.Events
{
    public class ValidCreatedEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
