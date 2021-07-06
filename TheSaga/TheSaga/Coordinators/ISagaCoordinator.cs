using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Options;
using TheSaga.States;

namespace TheSaga.Coordinators
{
    public interface ISagaCoordinator
    {
        Task<ISagaRunningState> GetSagaState(Guid id);
        Task<ISaga> Publish(ISagaEvent @event);
        Task<ISaga> Publish(ISagaEvent @event, IDictionary<string, object> executionValues);
        Task<ISaga> Publish(ISagaEvent @event, IDictionary<string, object> executionValues, SagaRunOptions runOptions);
        Task ResumeAll();
        Task Resume(Guid id);
        Task WaitForIdle(Guid id, SagaWaitOptions waitOptions = null);
    }

    internal interface ISagaInternalCoordinator
    {
        Task<ISaga> Publish(ISagaEvent @event, IDictionary<string, object> executionValues, Guid? parentId, SagaRunOptions runOptions);
    }
}
