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
        public String CurrentState;
        public SagaModel<TSagaData> Model;
        public IServiceProvider ServiceProvider;
        public UniqueNameGenerator UniqueNameGenerator;

        public SagaBuilderState(Type currentEvent, string currentState, SagaModel<TSagaData> model, IServiceProvider serviceProvider, UniqueNameGenerator uniqueNameGenerator)
        {
            this.CurrentEvent = currentEvent;
            this.CurrentState = currentState;
            this.Model = model;
            this.ServiceProvider = serviceProvider;
            this.UniqueNameGenerator = uniqueNameGenerator;
        }
    }
}