using System;
using TheSaga.Execution;
using TheSaga.Models;
using TheSaga.SagaModels;

namespace TheSaga.Registrator
{
    public interface ISagaRegistrator
    {
        ISagaModel FindModelForEventType(Type eventType);

        void Register<TSagaData>(ISagaModel<TSagaData> model)
            where TSagaData : ISagaData;

        // void Register(ISagaModel model, Type sagaData);

        //internal ISagaExecutor FindExecutorForStateType(Type stateType);
    }
}