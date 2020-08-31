using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncAndValid.Events
{
    public class CreatedEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}