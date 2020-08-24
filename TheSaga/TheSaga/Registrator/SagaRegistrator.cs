using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Interfaces;
using TheSaga.Models;

namespace TheSaga.Registrator
{
    public class SagaRegistrator : ISagaRegistrator
    {
        Dictionary<string, RegisteredSagaInfo> models;

        public SagaRegistrator()
        {
            models = new Dictionary<string, RegisteredSagaInfo>();
            //models = new Dictionary<string, SagaModel>();
        }

        public ISagaModel FindModel(IEvent @event)
        {
            return models.Values.
                FirstOrDefault(v => v.SagaModel.ContainsEvent(@event.GetType()))?.
                SagaModel;
        }

        public void Register(string sagaName, ISagaModel model) 
        {
            models[sagaName] = new RegisteredSagaInfo()
            {
                SagaModel = model,
                SagaName = sagaName
            };
        }
    }

    internal class RegisteredSagaInfo
    {
        internal string SagaName;

        internal ISagaModel SagaModel;
    }
}