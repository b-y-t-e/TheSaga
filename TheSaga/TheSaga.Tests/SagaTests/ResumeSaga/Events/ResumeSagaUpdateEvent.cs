using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.ResumeSaga.Events
{
    public class ResumeSagaUpdateEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
