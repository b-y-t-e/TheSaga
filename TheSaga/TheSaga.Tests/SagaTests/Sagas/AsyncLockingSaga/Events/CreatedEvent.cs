using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncLockingSaga.Events
{
    public class CreatedEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}