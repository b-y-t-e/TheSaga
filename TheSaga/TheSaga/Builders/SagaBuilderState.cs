using System;
using TheSaga.Models;
using TheSaga.SagaModels;
using TheSaga.Utils;

namespace TheSaga.Builders
{
    internal class SagaBuilderState<TSagaData>
        where TSagaData : ISagaData
    {
        public Type CurrentEvent;
        public string CurrentState;
        public SagaModel<TSagaData> Model;
        public IServiceProvider ServiceProvider;
        public UniqueNameGenerator UniqueNameGenerator;

        public SagaBuilderState(Type currentEvent, string currentState, SagaModel<TSagaData> model,
            IServiceProvider serviceProvider, UniqueNameGenerator uniqueNameGenerator)
        {
            CurrentEvent = currentEvent;
            CurrentState = currentState;
            Model = model;
            ServiceProvider = serviceProvider;
            UniqueNameGenerator = uniqueNameGenerator;
        }
    }
}