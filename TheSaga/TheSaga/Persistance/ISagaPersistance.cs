using System;
using System.Threading.Tasks;
using TheSaga.SagaStates;

namespace TheSaga.Persistance
{
    public interface ISagaPersistance
    {
        Task<ISaga> Get(Guid id);

        Task Remove(Guid id);

        Task Set(ISaga sagaData);
    }
}