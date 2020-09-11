using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.SyncAndValid.Events
{
    public class ToAlternative1Event : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}
