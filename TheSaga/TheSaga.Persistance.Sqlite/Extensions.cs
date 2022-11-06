using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using TheSaga.Config;
using TheSaga.Persistance.Sqlite.Connection;
using TheSaga.Persistance.Sqlite.Options;

[assembly: InternalsVisibleTo("TheSaga.Tests")]

namespace TheSaga.Persistance.Sqlite
{
    public static class Extensions
    {
        public static void UseSqlite(this ITheSagaConfig config, SqliteOptions options)
        {
            if (string.IsNullOrEmpty(options?.ConnectionString))
                throw new Exception($"ConnectionString for TheSaga.Persistance.Sqlite cannot be empty");

            config.Services.AddSingleton<SqliteOptions>(ctx => options);
            config.Services.AddTransient<ISagaPersistance, SqliteSagaPersistance>();
            config.Services.AddTransient<ISqliteConnection>(ctx =>
            {
                return new SqliteConnection(options.ConnectionString);
            });
        }
    }
}