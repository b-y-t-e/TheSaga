using System;
using System.Collections.Generic;
using TheSaga.SagaModels.Steps;

namespace TheSaga.SagaModels.Actions
{
    public interface ISagaActions : IEnumerable<ISagaAction>
    {
        void Add(ISagaAction action);
        bool IsStartEvent(Type eventType);
        bool IsEventSupported(Type eventType);
    }
}