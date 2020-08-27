using System;
using System.Collections.Generic;

namespace TheSaga.Utils
{
    public class SagaLocking : ISagaLocking
    {
        private HashSet<Guid> locks =
            new HashSet<Guid>();

        public bool Acquire(Guid guid)
        {
            lock (locks)
            {
                bool hasAcquired = !locks.Contains(guid);
                if (hasAcquired) locks.Add(guid);
                return hasAcquired;
            }
        }

        public bool Banish(Guid guid)
        {
            lock (locks)
            {
                bool hasBanished = locks.Contains(guid);
                if (hasBanished) locks.Remove(guid);
                return hasBanished;
            }
        }

        public bool IsAcquired(Guid guid)
        {
            lock (locks)
            {
                return locks.Contains(guid);
            }
        }
    }
}