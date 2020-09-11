using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.SyncAndInvalidSaga.Events
{
    public class ValidUpdateEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
