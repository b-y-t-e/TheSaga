using System;
using TheSaga.Models;
using TheSaga.SagaModels;

namespace TheSaga.Registrator
{
    public interface ISagaRegistrator
    {
        ISagaModel FindModelByName(string name);
        ISagaModel FindModelForEventType(Type eventType);
        void Register(ISagaModel model);
    }
}