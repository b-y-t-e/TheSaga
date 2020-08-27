using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndValid.Events
{
    public class ToAlternative2Event : IEvent
    {
        public Guid ID { get; set; }
    }
}