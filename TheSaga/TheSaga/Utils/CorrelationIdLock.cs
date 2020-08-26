using System;
using System.Collections.Generic;

namespace TheSaga.Utils
{
    internal static class CorrelationIdLock
    {
        static HashSet<Guid> locks =
            new HashSet<Guid>();

        internal static bool Acquire(this Guid guid)
        {
            lock(locks)
            {
                bool hasAcquired = !locks.Contains(guid);
                if (hasAcquired) locks.Add(guid);
                return hasAcquired;
            }
        }

        internal static bool IsAcquired(this Guid guid)
        {
            lock (locks)
            {
                return locks.Contains(guid);
            }
        }

        internal static bool Banish(this Guid guid)
        {
            lock (locks)
            {
                bool hasBanished = locks.Contains(guid);
                if (hasBanished) locks.Remove(guid);
                return hasBanished;
            }
        }
    }
}