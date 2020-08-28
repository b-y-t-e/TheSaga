using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.TransitionsSaga.Events
{
    public class InvalidUpdateEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}