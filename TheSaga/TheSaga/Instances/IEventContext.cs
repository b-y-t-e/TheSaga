﻿using TheSaga.Interfaces;
using TheSaga.States;

namespace TheSaga.Builders
{

    public interface IEventContext<TSagaState, TEvent> : IEventContext
        where TSagaState : ISagaState
       where TEvent : IEvent
    {
        public TSagaState State { get; set; }
        public TEvent Event { get; set; }
    }

    public interface IEventContext
    {
    }
}