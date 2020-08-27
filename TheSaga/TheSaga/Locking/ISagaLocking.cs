using System;

namespace TheSaga.Utils
{
    public interface ISagaLocking
    {
        bool Acquire(Guid guid);
        bool Banish(Guid guid);
        bool IsAcquired(Guid guid);
    }
}