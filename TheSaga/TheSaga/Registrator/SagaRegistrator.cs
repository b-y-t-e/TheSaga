using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Executors;
using TheSaga.Interfaces;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.States;

namespace TheSaga.Registrator
{
    public class SagaRegistrator : ISagaRegistrator
    {
        ISagaPersistance sagaPersistance;
        List<ISagaModel> registeredModels;
        Dictionary<Type, ISagaExecutor> registeredExecutors;

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