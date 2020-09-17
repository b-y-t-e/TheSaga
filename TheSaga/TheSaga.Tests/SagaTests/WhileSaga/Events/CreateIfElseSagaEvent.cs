using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.WhileSaga.Events
{
    public class CreateWhileSagaEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
