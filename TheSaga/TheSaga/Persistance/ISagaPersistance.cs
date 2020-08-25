using System;
using System.Threading.Tasks;
using TheSaga.SagaStates;

namespace TheSaga.Persistance
{
    public interface ISagaPersistance
    {
        Task<ISagaState> Get(Guid correlationID);

        Task Set(ISagaState sagaState);
        Task Remove(Guid correlationID);
    }
}