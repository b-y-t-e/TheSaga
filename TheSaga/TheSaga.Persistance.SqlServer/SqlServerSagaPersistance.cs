using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Persistance.SqlServer.Connection;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Persistance.SqlServer.Utils;
using TheSaga.Providers;
using TheSaga.SagaStates;

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

        public async Task<ISagaState> Get(Guid correlationID)
        {
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions))
            {
                return await sagaStore.Get(correlationID);
            }
        }

        public async Task Remove(Guid correlationID)
        {
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions))
            {
                await sagaStore.Remove(correlationID);
            }
        }

        public async Task Set(ISagaState sagaState)
        {
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions))
            {
                await sagaStore.Store(sagaState);
            }
        }

    }
}