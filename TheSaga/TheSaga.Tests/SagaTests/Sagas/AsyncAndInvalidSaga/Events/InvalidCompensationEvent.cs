﻿using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga.Events
{
    public class InvalidCompensationEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}