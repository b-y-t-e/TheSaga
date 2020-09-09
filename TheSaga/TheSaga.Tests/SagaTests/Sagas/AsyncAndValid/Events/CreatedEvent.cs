using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncAndValid.Events
{
    public class CreatedEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}