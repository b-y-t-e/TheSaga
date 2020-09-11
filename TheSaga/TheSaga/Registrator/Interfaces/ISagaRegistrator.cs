using System;
using TheSaga.ModelsSaga.Interfaces;

namespace TheSaga.Registrator.Interfaces
{
    public interface ISagaRegistrator
    {
        internal ISagaModel FindModelByName(string name);
        internal ISagaModel FindModelForEventType(Type eventType);
        void Register(ISagaModel model);
    }
}
