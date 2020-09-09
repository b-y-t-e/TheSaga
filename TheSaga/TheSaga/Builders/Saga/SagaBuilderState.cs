using System;
using TheSaga.Models;
using TheSaga.SagaModels;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.Steps;
using TheSaga.Utils;

namespace TheSaga.Builders
{
    internal class SagaBuilderState
    {
        public ISagaStep ParentStep;
        public Type CurrentEvent;
        public string CurrentState;
        public ISagaModel Model;
        public IServiceProvider ServiceProvider;
        public UniqueNameGenerator UniqueNameGenerator;
        public SagaAction CurrentAction;

        public SagaBuilderState(Type currentEvent, string currentState, ISagaModel model,
            IServiceProvider serviceProvider, UniqueNameGenerator uniqueNameGenerator, ISagaStep parentStep)
        {
            CurrentEvent = currentEvent;
            CurrentState = currentState;
            Model = model;
            ServiceProvider = serviceProvider;
            UniqueNameGenerator = uniqueNameGenerator;
            ParentStep = parentStep;
        }
    }
}