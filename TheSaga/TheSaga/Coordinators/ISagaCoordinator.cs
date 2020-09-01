﻿using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Options;
using TheSaga.States;

namespace TheSaga.Coordinators
{
    public interface ISagaCoordinator
    {
        Task<ISaga> Publish(IEvent @event);
        Task ResumeAll();
        Task Resume(Guid id);
        Task WaitForIdle(Guid id, SagaWaitOptions waitOptions = null);
    }
}