using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.MoveToSaga.Events
{
    public class CreateMoveToSaga : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
