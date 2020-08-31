using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.SendTests.Events
{
    public class SendAlternativeCreateEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}