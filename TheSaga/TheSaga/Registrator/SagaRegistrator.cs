using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheSaga.Messages.MessageBus;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Providers;
using TheSaga.SagaModels;
using TheSaga.Utils;

namespace TheSaga.Registrator
{
    public class SagaRegistrator : ISagaRegistrator
    {
        private IMessageBus internalMessageBus;
        private IServiceProvider serviceProvider;

        private List<ISagaModel> registeredModels;
        private bool wasInitialized;

        public SagaRegistrator(
            IMessageBus internalMessageBus,
            IServiceProvider serviceProvider)
        {
            this.registeredModels = new List<ISagaModel>();
            this.internalMessageBus = internalMessageBus;
            this.serviceProvider = serviceProvider;
            RegisterAllModelWithBuilders();
        }

        public ISagaModel FindModelByName(string name)
        {
            return registeredModels.
                FirstOrDefault(v => v.Name == name);
        }

        public void Register(ISagaModel model)
        {
            registeredModels.
                Add(model);
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

                Register(model);
            }

            wasInitialized = true;
        }

    }
}