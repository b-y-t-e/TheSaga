using System;
using System.Collections.Generic;
using TheSaga.States;
using TheSaga.States.Actions;

namespace TheSaga.Models
{
    public interface ISagaModel<TSagaState> : ISagaModel where TSagaState : ISagaState
    {
        SagaActions<TSagaState> Actions { get; }
    }

    public interface ISagaModel
    {
        Type SagaStateType { get; }
        bool IsStartEvent(Type type);
        bool ContainsEvent(Type type);
        ISagaAction FindAction(string state, Type eventType);
        IList<ISagaAction> FindActions(string state);
    }
}