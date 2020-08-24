using System;
using System.Threading.Tasks;
using TheSaga.Instances;

namespace TheSaga.Seekers
{
    public interface ISagaSeeker
    {
        Task<ISagaInstance> Seek(Guid correlationID);
    }
}