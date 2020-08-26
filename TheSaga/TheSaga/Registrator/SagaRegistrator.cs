using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Execution;
using TheSaga.InternalMessages.MessageBus;
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
        IInternalMessageBus internalMessageBus;

        public SagaRegistrator(ISagaPersistance sagaPersistance, IInternalMessageBus internalMessageBus)
        {
            this.registeredExecutors = new Dictionary<Type, ISagaExecutor>();
            this.registeredModels = new List<ISagaModel>();
            this.sagaPersistance = sagaPersistance;
            this.internalMessageBus = internalMessageBus;
        }

        ISagaExecutor ISagaRegistrator.FindExecutorForStateType(Type stateType)
        {
            ISagaExecutor sagaExecutor = null;
            registeredExecutors.TryGetValue(stateType, out sagaExecutor);
            return sagaExecutor;
        }

        ISagaModel ISagaRegistrator.FindModelForEventType(Type eventType)
        {
            return registeredModels.
                FirstOrDefault(v => v.ContainsEvent(eventType));
        }

        public void Register<TSagaState>(ISagaModel<TSagaState> model)
            where TSagaState : ISagaState
        {
            registeredModels.Add((ISagaModel)model);

            registeredExecutors[typeof(TSagaState)] =
                new SagaExecutor<TSagaState>(model, sagaPersistance, internalMessageBus);
        }

    }
}