﻿using System;
using TheSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.ResumeSaga.Events
{
    public class ResumeSagaCreateEvent : ISagaEvent
    {
        public Guid ID { get; set; }
    }
}