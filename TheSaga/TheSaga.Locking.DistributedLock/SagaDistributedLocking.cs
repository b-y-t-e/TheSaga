using Medallion.Threading;
using Medallion.Threading.SqlServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Locking.DistributedLock.Options;

namespace TheSaga.Locking.DistributedLock
{
    internal class SagaDistributedLocking : ISagaLocking
    {
        SqlServerLockingOptions lockingOptions;
        Dictionary<Guid, IDisposable> locks;

        public SagaDistributedLocking(SqlServerLockingOptions lockingOptions)
        {
            this.lockingOptions = lockingOptions;
            this.locks = new Dictionary<Guid, IDisposable>();
        }

        public async Task<bool> Acquire(Guid guid)
        {
            SqlDistributedLock sqlDistributedLock =
                new SqlDistributedLock($"TheSaga-{guid}", lockingOptions.ConnectionString);

            IDisposable handle = sqlDistributedLock.
                TryAcquire(TimeSpan.FromSeconds(1));

            if (handle == null)
                return false;

            lock (locks)
                locks[guid] = handle;
            return true;
        }

        public async Task<bool> IsAcquired(Guid guid)
        {
            bool acquired = await Acquire(guid);
            if (!acquired)
                return true;

            await Banish(guid);
            return false;
        }

        public async Task<bool> Banish(Guid guid)
        {
            lock (locks)
            {
                if (locks.ContainsKey(guid))
                {
                    locks[guid].Dispose();
                    locks.Remove(guid);
                }
            }
            return false;
        }

    }
}