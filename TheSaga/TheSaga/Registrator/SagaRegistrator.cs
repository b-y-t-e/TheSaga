using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Execution;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.SagaStates;

namespace TheSaga.Registrator
{
    public class SagaRegistrator : ISagaRegistrator
    {
        Dictionary<Type, ISagaExecutor> registeredExecutors;
        List<ISagaModel> registeredModels;
        ISagaPersistance sagaPersistance;

        public SagaRegistrator(ISagaPersistance sagaPersistance)
        {
            this.registeredExecutors = new Dictionary<Type, ISagaExecutor>();
            this.registeredModels = new List<ISagaModel>();
            this.sagaPersistance = sagaPersistance;
        }

        public ISagaExecutor FindExecutorForStateType(Type stateType)
        {
            ISagaExecutor sagaExecutor = null;
            registeredExecutors.TryGetValue(stateType, out sagaExecutor);
            return sagaExecutor;
        }

        public ISagaModel FindModelForEventType(Type eventType)
        {
            return registeredModels.
                FirstOrDefault(v => v.ContainsEvent(eventType));
        }

        public void Register<TSagaState>(ISagaModel<TSagaState> model)
            where TSagaState : ISagaState
        {
            registeredModels.Add((ISagaModel)model);
            registeredExecutors[typeof(TSagaState)] = new SagaExecutor<TSagaState>(sagaPersistance);
        }
    }
}