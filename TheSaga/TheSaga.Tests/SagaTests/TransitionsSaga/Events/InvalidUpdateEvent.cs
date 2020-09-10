using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.TransitionsSaga.Events
{
    public class InvalidUpdateEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}