using System;
using System.Threading.Tasks;

namespace TheSaga.Locking
{
    public interface ISagaLocking
    {
        Task<bool> Acquire(Guid guid);
        Task<bool> Banish(Guid guid);
        Task<bool> IsAcquired(Guid guid);
    }
}