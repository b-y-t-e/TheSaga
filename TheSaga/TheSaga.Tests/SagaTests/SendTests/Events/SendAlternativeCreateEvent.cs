using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.SendTests.Events
{
    public class SendAlternativeCreateEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
