using System;
using System.Collections.Generic;
using TheSaga.Model;

namespace TheSaga
{
    public class SagaRegistrator : ISagaRegistrator
    {
        Dictionary<string, RegisteredSagaInfo> models;

        public SagaRegistrator()
        {
            models = new Dictionary<string, RegisteredSagaInfo>();
            //models = new Dictionary<string, SagaModel>();
        }

        /*public void Register<TSagaType, TState>(string sagaName) 
            where TSagaType : ISaga<TState>
            where TState : IData
        {
            ISagaBuilder<TSagaType> sagaBuilder = new SagaBuilder();
        }*/

        public void Register<TSagaState>(string sagaName, SagaModel<TSagaState> model) where TSagaState : ISagaState
        {
            models[sagaName] = new RegisteredSagaInfo()
            {
                SagaModel = model,
                SagaName = sagaName,
                SagaStateType = typeof(TSagaState)
            };
        }
    }

    internal class RegisteredSagaInfo
    {
        internal string SagaName;

        internal ISagaModel SagaModel;

        internal Type SagaStateType;
    }
}