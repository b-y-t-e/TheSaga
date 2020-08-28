using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncLockingSaga.Events
{
    public class CreatedEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}