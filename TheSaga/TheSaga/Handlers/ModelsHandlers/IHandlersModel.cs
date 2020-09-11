using System;

namespace TheSaga.Handlers.ModelsHandlers
{
    public interface IHandlersModel
    {
        // SagaActions<TSagaData> Actions { get; }

        Type SagaStateType { get; }

        string Name { get; }

        bool ContainsEvent(Type type);

        /* ISagaAction FindActionForStep(ISagaStep sagaStep);

         ISagaAction FindActionForStateAndEvent(string state, Type eventType);

         ISagaStep FindStep(ISaga saga, Type eventType);

         bool IsStartEvent(Type type);  */
    }
}
