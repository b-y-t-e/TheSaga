﻿using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.ChildStepsSaga.Events
{
    public class SagaCreateWithDoStepEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}