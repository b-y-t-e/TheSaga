using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.ResumeSaga.Events
{
    public class InvalidCreateEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
