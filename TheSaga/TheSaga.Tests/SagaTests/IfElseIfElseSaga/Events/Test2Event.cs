﻿using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.IfElseIfElseSaga.Events
{
    public class Test2Event : ISagaEvent
    {
        public Guid ID { get; set; }
        public int Condition { get; set; }
    }
}