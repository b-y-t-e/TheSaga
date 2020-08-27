using System;
using TheSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndValid.Events
{
    public class ToAlternative1Event : IEvent
    {
        public Guid ID { get; set; }
    }
}