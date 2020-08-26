using System;
using TheSaga.Execution;
using TheSaga.Models;
using TheSaga.SagaStates;

namespace TheSaga.Registrator
{
    public interface ISagaRegistrator
    {
        ISagaModel FindModelForEventType(Type eventType);

        void Register<TSagaState>(ISagaModel<TSagaState> model)
            where TSagaState : ISagaState;

        internal ISagaExecutor FindExecutorForStateType(Type stateType);
    }
}