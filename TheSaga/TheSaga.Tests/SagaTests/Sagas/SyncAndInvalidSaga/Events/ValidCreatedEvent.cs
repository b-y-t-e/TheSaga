using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndInvalidSaga.Events
{
    public class ValidCreatedEvent : IEvent
    {
        public Guid ID { get; set; }
    }
}