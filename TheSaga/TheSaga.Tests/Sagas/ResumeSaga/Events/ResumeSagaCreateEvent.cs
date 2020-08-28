using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.ResumeSaga.Events
{
    public class ResumeSagaCreateEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}