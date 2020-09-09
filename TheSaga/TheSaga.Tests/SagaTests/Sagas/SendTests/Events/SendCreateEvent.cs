using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.SendTests.Events
{
    public class SendCreateEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}