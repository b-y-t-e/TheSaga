using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using TheSaga.Config;
using TheSaga.Locking.DistributedLock.Options;

[assembly: InternalsVisibleTo("TheSaga.Tests")]

namespace TheSaga.Locking.DistributedLock
{
    public static class Extensions
    {
        public static void UseDistributedLock(this ITheSagaConfig config, SqlServerLockingOptions options)
        {
            if (string.IsNullOrEmpty(options?.ConnectionString))
                throw new Exception($"ConnectionString for TheSaga.Locking.DistributedLock cannot be empty");

            config.Services.AddSingleton<SqlServerLockingOptions>(ctx => options);
            config.Services.AddSingleton<ISagaLocking, SagaDistributedLocking>();          
        }
    }
}