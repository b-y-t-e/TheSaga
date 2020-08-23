using System;
using System.Threading.Tasks;

namespace TheSaga
{
    public interface ISagaSeeker
    {
        Task<ISagaInstance> Seek(Guid correlationID);
    }
}