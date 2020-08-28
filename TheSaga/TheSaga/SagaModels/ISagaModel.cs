using System;
using System.Collections.Generic;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.Steps;

namespace TheSaga.SagaModels
{
    public interface ISagaModel<TSagaData> : ISagaModel where TSagaData : ISagaData
    {
        SagaActions<TSagaData> Actions { get; }
    }

    public interface ISagaModel
    {
        Type SagaStateType { get; }

        bool ContainsEvent(Type type);

        string Name { get; }

        ISagaAction FindActionForStep(ISagaStep sagaStep);

        ISagaAction FindActionForStateAndEvent(string state, Type eventType);

        IList<ISagaAction> FindActionsForState(string state);

        bool IsStartEvent(Type type);
    }
}