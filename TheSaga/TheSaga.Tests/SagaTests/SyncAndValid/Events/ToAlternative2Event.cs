using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.SyncAndValid.Events
{
    public class ToAlternative2Event : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
