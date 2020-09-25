using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.WhileSaga.Events
{
    public class Test3Event : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
