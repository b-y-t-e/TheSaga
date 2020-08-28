using System;
using TheSaga.SagaModels;

namespace TheSaga.Registrator
{
    public interface ISagaRegistrator
    {
        internal ISagaModel FindModelByName(string name);
        internal ISagaModel FindModelForEventType(Type eventType);
        void Register(ISagaModel model);
    }
}