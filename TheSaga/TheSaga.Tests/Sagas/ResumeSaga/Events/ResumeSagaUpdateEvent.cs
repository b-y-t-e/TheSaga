using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.ResumeSaga.Events
{
    public class ResumeSagaUpdateEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}