using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga.Events
{
    public class ValidUpdateEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}