using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndValid.Events
{
    public class ToAlternative2Event : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}