using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.WhileSaga.Events
{
    public class Test1Event : ISagaEvent
    {
        public Guid ID { get; set; }
        public int Counter { get; set; }
    }
}
