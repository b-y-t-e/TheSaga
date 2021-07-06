using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.ResumeSaga.Events
{
    public class CreateNewSagaEvent : ISagaEvent
    {
        public Guid ID { get; set; }
        public Guid NewID { get; set; }
    }
}
