using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.TransitionsSaga.Events
{
    public class CreateEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}