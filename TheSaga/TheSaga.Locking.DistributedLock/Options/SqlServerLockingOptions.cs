using System;

namespace TheSaga.Locking.DistributedLock.Options
{
    public class SqlServerLockingOptions
    {
        public SqlServerLockingOptions() 
        {

        }

        public String ConnectionString { get; set; }
    }
}