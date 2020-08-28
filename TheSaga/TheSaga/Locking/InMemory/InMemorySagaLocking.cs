using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheSaga.Locking.InMemory
{
    public class InMemorySagaLocking : ISagaLocking
    {
        private readonly HashSet<Guid> locks =
            new HashSet<Guid>();

        public Task<bool> Acquire(Guid guid)
        {
            lock (locks)
            {
                var hasAcquired = !locks.Contains(guid);
                if (hasAcquired) locks.Add(guid);
                return Task.FromResult(hasAcquired);
            }
        }

        public Task<bool> Banish(Guid guid)
        {
            lock (locks)
            {
                var hasBanished = locks.Contains(guid);
                if (hasBanished) locks.Remove(guid);
                return Task.FromResult(hasBanished);
            }
        }

        public Task<bool> IsAcquired(Guid guid)
        {
            lock (locks)
            {
                return Task.FromResult(
                    locks.Contains(guid));
            }
        }
    }
}