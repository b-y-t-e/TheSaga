using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.ResumeSaga.Events
{
    public class ResumeSagaUpdateEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}