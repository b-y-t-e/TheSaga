using System;
using TheSaga.Execution;
using TheSaga.Models;
using TheSaga.SagaStates;

namespace TheSaga.Registrator
{
    public interface ISagaRegistrator
    {
        ISagaExecutor FindExecutorForStateType(Type stateType);

        ISagaModel FindModelForEventType(Type eventType);

        void Register<TSagaState>(ISagaModel<TSagaState> model)
            where TSagaState : ISagaState;
    }
}