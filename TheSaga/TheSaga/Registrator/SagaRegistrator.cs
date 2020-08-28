using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheSaga.Execution;
using TheSaga.Execution.AsyncHandlers;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Providers;
using TheSaga.SagaStates;
using TheSaga.Utils;

namespace TheSaga.Registrator
{
    public class SagaRegistrator : ISagaRegistrator
    {
        private IInternalMessageBus internalMessageBus;
        private Dictionary<Type, ISagaExecutor> registeredExecutors;
        private List<ISagaModel> registeredModels;
        private IServiceProvider serviceProvider;
        private bool wasInitialized;

        public SagaRegistrator(
            IInternalMessageBus internalMessageBus,
            IServiceProvider serviceProvider)
        {
            this.registeredExecutors = new Dictionary<Type, ISagaExecutor>();
            this.registeredModels = new List<ISagaModel>();
            this.internalMessageBus = internalMessageBus;
            this.serviceProvider = serviceProvider;
            RegisterAllModelWithBuilders();
        }


        public void Register<TSagaData>(ISagaModel<TSagaData> model)
            where TSagaData : ISagaData
        {
            Register(
                model,
                typeof(TSagaData));
        }

        public void Register(ISagaModel model, Type sagaDataType)
        {
            registeredModels.
                Add(model);

            Type sagaExecuterType = typeof(SagaExecutor<>).
                ConstructGenericType(sagaDataType);

            Type asyncStepCompletedObservableType = typeof(SagaAsyncStepCompletedObservable<>).
                ConstructGenericType(sagaDataType);

            ISagaExecutor sagaExecutor = (ISagaExecutor)ActivatorUtilities.
               CreateInstance(serviceProvider, sagaExecuterType, model);

            IObservable asyncStepCompletedObservable = (IObservable)ActivatorUtilities.
               CreateInstance(serviceProvider, asyncStepCompletedObservableType, sagaExecutor);

            registeredExecutors[model.GetType()] = sagaExecutor;
            asyncStepCompletedObservable.Subscribe();
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

        void RegisterAllModelWithBuilders()
        {
            if (wasInitialized)
                return;

            Type[] modelBuildersTypes = AppDomain.CurrentDomain.GetAssemblies().
                SelectMany(a => a.GetTypes()).
                Where(t => t.IsClass && t.Is(typeof(ISagaModelBuilder<>))).
                ToArray();

            foreach (Type modelBuilderType in modelBuildersTypes)
            {
                object modelBuilder = ActivatorUtilities.
                   CreateInstance(serviceProvider, modelBuilderType);

                ISagaModelBuilder<ISagaData> emptyBuildModel = ((ISagaModelBuilder<ISagaData>)null);
                string buildMethodName = nameof(emptyBuildModel.Build);
                MethodInfo buildMethodInfo = modelBuilderType.GetMethod(buildMethodName, BindingFlags.Public | BindingFlags.Instance);
                ISagaModel model = (ISagaModel)buildMethodInfo.Invoke(modelBuilder, new object[0]);

                Type sagaDataType = modelBuilderType.
                    GetInterfaceOf(typeof(ISagaModelBuilder<>)).
                    GetFirstGenericArgument();

                Register(
                    model,
                    sagaDataType);
            }

            wasInitialized = true;
        }

    }
}