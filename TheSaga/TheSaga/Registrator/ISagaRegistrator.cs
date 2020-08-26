using System;
using TheSaga.Execution;
using TheSaga.Models;
using TheSaga.SagaStates;

namespace TheSaga.Registrator
{
    public interface ISagaRegistrator
    {
        ISagaModel FindModelForEventType(Type eventType);

        void Register<TSagaData>(ISagaModel<TSagaData> model)
            where TSagaData : ISagaData;

        internal ISagaExecutor FindExecutorForStateType(Type stateType);
    }
}