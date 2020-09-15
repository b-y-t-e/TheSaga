using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.ResumeSaga.Events
{
    public class CreateEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
