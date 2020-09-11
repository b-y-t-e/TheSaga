using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.TransitionsSaga.Events
{
    public class CreateEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
