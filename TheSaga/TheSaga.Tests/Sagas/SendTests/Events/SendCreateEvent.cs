using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SendTests.Events
{
    public class SendCreateEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}