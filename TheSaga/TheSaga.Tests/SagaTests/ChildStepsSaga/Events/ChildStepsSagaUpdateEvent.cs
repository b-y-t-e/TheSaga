using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.ChildStepsSaga.Events
{
    public class ChildStepsSagaUpdateEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
