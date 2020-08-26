using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Execution;
using TheSaga.Execution.AsyncHandlers;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Providers;
using TheSaga.SagaStates;

namespace TheSaga.Registrator
{
    public class SagaRegistrator : ISagaRegistrator
    {
        private IInternalMessageBus internalMessageBus;
        private Dictionary<Type, ISagaExecutor> registeredExecutors;
        private List<ISagaModel> registeredModels;
        private IServiceProvider serviceProvider;

        public SagaRegistrator(
            IInternalMessageBus internalMessageBus,
            IServiceProvider serviceProvider)
        {
            this.registeredExecutors = new Dictionary<Type, ISagaExecutor>();
            this.registeredModels = new List<ISagaModel>();
            this.internalMessageBus = internalMessageBus;
            this.serviceProvider = serviceProvider;
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

        public void Register<TSagaData>(ISagaModel<TSagaData> model)
            where TSagaData : ISagaData
        {
            registeredModels.
                Add((ISagaModel)model);

            SagaExecutor<TSagaData> sagaExecutor = ActivatorUtilities.
               CreateInstance<SagaExecutor<TSagaData>>(serviceProvider, model);

            new SagaAsyncStepCompletedHandler<TSagaData>(sagaExecutor, internalMessageBus).
                Subscribe();

            registeredExecutors[typeof(TSagaData)] = sagaExecutor;
        }
    }
}