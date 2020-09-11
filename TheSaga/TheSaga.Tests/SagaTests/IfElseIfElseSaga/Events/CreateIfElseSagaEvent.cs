using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.IfElseIfElseSaga.Events
{
    public class CreateIfElseSagaEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}