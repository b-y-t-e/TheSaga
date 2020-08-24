using System;
using System.Threading.Tasks;
using TheSaga.States;

namespace TheSaga.Persistance
{
    public interface ISagaPersistance
    {
        Task<ISagaState> Get(Guid correlationID);

        Task Set(ISagaState sagaState);
    }
}