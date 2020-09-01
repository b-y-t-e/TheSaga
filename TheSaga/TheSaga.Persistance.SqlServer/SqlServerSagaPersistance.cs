using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Models;
using TheSaga.Persistance.SqlServer.Connection;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Persistance.SqlServer.Utils;
using TheSaga.Providers;

namespace TheSaga.Persistance.SqlServer
{
    public class SqlServerSagaPersistance : ISagaPersistance
    {
        Dictionary<Guid, string> instances;
        ISqlServerConnection sqlServerConnection;
        IDateTimeProvider dateTimeProvider;
        SqlServerOptions sqlServerOptions;

        public SqlServerSagaPersistance(ISqlServerConnection sqlServerConnection, IDateTimeProvider dateTimeProvider, SqlServerOptions sqlServerOptions)
        {
            this.instances = new Dictionary<Guid, string>();
            this.sqlServerConnection = sqlServerConnection;
            this.dateTimeProvider = dateTimeProvider;
            this.sqlServerOptions = sqlServerOptions;
        }

        public async Task<ISaga> Get(Guid id)
        {
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions))
            {
                return await sagaStore.Get(id);
            }
        }

        public Task<IList<Guid>> GetUnfinished()
        {
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions))
            {
                return sagaStore.GetUnfinished();
            }
        }

        public async Task Remove(Guid id)
        {
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions))
            {
                await sagaStore.Remove(id);
            }
        }

        public async Task Set(ISaga saga)
        {
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions))
            {
                await sagaStore.Store(saga);
            }
        }

    }
}