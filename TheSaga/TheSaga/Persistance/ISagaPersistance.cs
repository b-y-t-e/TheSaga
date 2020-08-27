using System;
using System.Threading.Tasks;
using TheSaga.SagaStates;

namespace TheSaga.Persistance
{
    public interface ISagaPersistance
    {
        Task<ISaga> Get(Guid correlationID);

        Task Remove(Guid correlationID);

        Task Set(ISaga sagaData);
    }
}