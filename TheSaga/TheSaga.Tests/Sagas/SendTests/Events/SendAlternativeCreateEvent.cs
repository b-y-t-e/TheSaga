using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SendTests
{
    public class SendAlternativeCreateEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}