using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.ChildStepsSaga.Events
{
    public class SagaCreateWithDoStepEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
