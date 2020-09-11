using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.MessageBus;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.Registrator.Interfaces;
using TheSaga.Utils;

namespace TheSaga.Registrator
{
    public class SagaRegistrator : ISagaRegistrator
    {
        private readonly List<ISagaModel> registeredModels;
        private readonly IServiceProvider serviceProvider;
        private IMessageBus messageBus;
        private bool wasInitialized;

        public SagaRegistrator(
            IMessageBus messageBus,
            IServiceProvider serviceProvider)
        {
            registeredModels = new List<ISagaModel>();
            this.messageBus = messageBus;
            this.serviceProvider = serviceProvider;
            RegisterAllModelWithBuilders();
        }

        public void Register(ISagaModel model)
        {
            registeredModels.Add(model);
        }

        ISagaModel ISagaRegistrator.FindModelForEventType(Type eventType)
        {
            return registeredModels.
                FirstOrDefault(model => model.Actions.IsEventSupported(eventType));
        }

        ISagaModel ISagaRegistrator.FindModelByName(string name)
        {
            return registeredModels.
                FirstOrDefault(model => model.Name == name);
        }

        private void RegisterAllModelWithBuilders()
        {
            if (wasInitialized)
                return;

            Type[] modelBuildersTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && t.Is(typeof(ISagaModelBuilder<>))).ToArray();

            foreach (Type modelBuilderType in modelBuildersTypes)
            {
                object modelBuilder = ActivatorUtilities.CreateInstance(serviceProvider, modelBuilderType);

                ISagaModelBuilder<ISagaData> emptyBuildModel = null;
                string buildMethodName = nameof(emptyBuildModel.Build);
                MethodInfo? buildMethodInfo =
                    modelBuilderType.GetMethod(buildMethodName, BindingFlags.Public | BindingFlags.Instance);
                ISagaModel model = (ISagaModel) buildMethodInfo.Invoke(modelBuilder, new object[0]);

                Register(model);
            }

            wasInitialized = true;
        }
    }
}
