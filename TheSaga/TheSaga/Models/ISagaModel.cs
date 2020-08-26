using System;
using System.Collections.Generic;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;

namespace TheSaga.Models
{
    public interface ISagaModel<TSagaData> : ISagaModel where TSagaData : ISagaData
    {
        SagaActions<TSagaData> Actions { get; }
    }

    public interface ISagaModel
    {
        Type SagaStateType { get; }

        bool ContainsEvent(Type type);

        ISagaAction FindActionForStep(ISagaStep sagaStep);

        ISagaAction FindActionOrCreateForStateAndEvent(string state, Type eventType);

        IList<ISagaAction> FindActionsForState(string state);

        bool IsStartEvent(Type type);
    }
}