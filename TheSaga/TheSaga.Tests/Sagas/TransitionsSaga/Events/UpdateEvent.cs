using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.TransitionsSaga.Events
{
    public class UpdateEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}