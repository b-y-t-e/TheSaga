using System;
using System.Threading.Tasks;

namespace TheSaga.Utils
{
    public interface ISagaLocking
    {
        Task<bool> Acquire(Guid guid);
        Task<bool> Banish(Guid guid);
        Task<bool> IsAcquired(Guid guid);
    }
}