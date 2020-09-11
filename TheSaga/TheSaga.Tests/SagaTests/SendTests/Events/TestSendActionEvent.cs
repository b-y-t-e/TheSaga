using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.SendTests.Events
{
    public class TestSendActionEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
