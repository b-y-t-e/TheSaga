using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.ChildStepsSaga.Events
{
    public class SagaCreateWithIfStepEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
