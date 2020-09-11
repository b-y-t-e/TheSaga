using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.IfElseSaga.Events
{
    public class Test1Event : ISagaEvent
    {
        public Guid ID { get; set; }
        public int Condition { get; set; }
    }
}
