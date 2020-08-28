using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndValid.Events
{
    public class ToAlternative2Event : IEvent
    {
        public Guid ID { get; set; }
    }
}