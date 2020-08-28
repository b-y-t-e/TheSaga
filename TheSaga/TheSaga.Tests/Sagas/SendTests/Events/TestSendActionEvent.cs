using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SendTests.Events
{
    public class TestSendActionEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}