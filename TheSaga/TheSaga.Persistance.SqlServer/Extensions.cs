using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using TheSaga.Config;
using TheSaga.Persistance.SqlServer.Connection;
using TheSaga.Persistance.SqlServer.Options;

[assembly: InternalsVisibleTo("TheSaga.Tests")]

namespace TheSaga.Persistance.SqlServer
{
    public static class Extensions
    {
        public static void UseSqlServer(this ITheSagaConfig config, SqlServerOptions options)
        {
            if (string.IsNullOrEmpty(options?.ConnectionString))
                throw new Exception($"ConnectionString for TheSaga.Persistance.SqlServer cannot be empty");

            config.Services.AddSingleton<SqlServerOptions>(ctx => options);
            config.Services.AddTransient<ISagaPersistance, SqlServerSagaPersistance>();
            config.Services.AddTransient<ISqlServerConnection>(ctx =>
            {
                return new SqlServerConnection(options.ConnectionString);
            });
        }
    }
}