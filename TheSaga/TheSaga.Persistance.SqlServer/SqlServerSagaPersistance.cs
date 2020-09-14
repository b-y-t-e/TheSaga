using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance.InMemory;
using TheSaga.Persistance.SqlServer.Connection;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Persistance.SqlServer.Utils;
using TheSaga.Providers;
using TheSaga.Providers.Interfaces;

namespace TheSaga.Persistance.SqlServer
{
    public class SqlServerSagaPersistance : ISagaPersistance
    {
        Dictionary<Guid, string> instances;
        ISqlServerConnection sqlServerConnection;
        IDateTimeProvider dateTimeProvider;
        SqlServerOptions sqlServerOptions;
        InMemorySagaPersistance inMemorySagaPersistance;

        public SqlServerSagaPersistance(ISqlServerConnection sqlServerConnection, IDateTimeProvider dateTimeProvider, SqlServerOptions sqlServerOptions)
        {
            this.instances = new Dictionary<Guid, string>();
            this.sqlServerConnection = sqlServerConnection;
            this.dateTimeProvider = dateTimeProvider;
            this.sqlServerOptions = sqlServerOptions;
            this.inMemorySagaPersistance = new InMemorySagaPersistance();
        }

        public async Task<ISaga> Get(Guid id)
        {
            ISaga saga = await inMemorySagaPersistance.Get(id);
            if (saga != null)
                return saga;

            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions))            
                return await sagaStore.Get(id);            
        }

        public Task<IList<Guid>> GetUnfinished()
        {
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions))            
                return sagaStore.GetUnfinished();            
        }

        public async Task Remove(Guid id)
        {
            await inMemorySagaPersistance.Remove(id);
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions))            
                await sagaStore.Remove(id);            
        }

        public async Task Set(ISaga saga)
        {
            await inMemorySagaPersistance.Set(saga);
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions))            
                await sagaStore.Store(saga);            
        }

    }
}
