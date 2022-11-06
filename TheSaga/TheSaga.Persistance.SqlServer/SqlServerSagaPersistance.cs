using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Messages;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance.InMemory;
using TheSaga.Persistance.SqlServer.Connection;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Persistance.SqlServer.Utils;
using TheSaga.Providers;
using TheSaga.Providers.Interfaces;
using TheSaga.Serializer;

namespace TheSaga.Persistance.SqlServer
{
    public class SqlServerSagaPersistance : ISagaPersistance
    {
        private readonly IMessageBus messageBus;
        Dictionary<Guid, string> instances;
        ISqlServerConnection sqlServerConnection;
        IDateTimeProvider dateTimeProvider;
        SqlServerOptions sqlServerOptions;
        WeakInMemorySagaPersistance weakInMemorySagaPersistance;
        private readonly ISagaSerializer sagaSerializer;

        public SqlServerSagaPersistance(ISqlServerConnection sqlServerConnection, IDateTimeProvider dateTimeProvider, SqlServerOptions sqlServerOptions, IMessageBus messageBus, ISagaSerializer sagaSerializer)
        {
            this.instances = new Dictionary<Guid, string>();
            this.sqlServerConnection = sqlServerConnection;
            this.dateTimeProvider = dateTimeProvider;
            this.sqlServerOptions = sqlServerOptions;
            this.weakInMemorySagaPersistance = new WeakInMemorySagaPersistance(
                TimeSpan.FromSeconds(15), sagaSerializer);
            this.messageBus = messageBus;
            this.sagaSerializer = sagaSerializer;
        }

        public async Task<ISaga> Get(Guid id)
        {
            /*ISaga saga = await weakInMemorySagaPersistance.Get(id);
            if (saga != null)
                return saga;*/

            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions, sagaSerializer))
            {
                var saga = await sagaStore.Get(id);
                
                await messageBus.
                    Publish(new SagaAfterRetrivedMessage(saga));

                return saga;
            }
        }

        public Task<IList<Guid>> GetUnfinished()
        {
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions, sagaSerializer))
                return sagaStore.GetUnfinished();
        }

        public async Task Remove(Guid id)
        {
            await weakInMemorySagaPersistance.Remove(id);
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions, sagaSerializer))
                await sagaStore.Remove(id);
        }

        public async Task Set(ISaga saga)
        {
            await weakInMemorySagaPersistance.Set(saga);
            using (SagaStore sagaStore = new SagaStore(sqlServerConnection, dateTimeProvider, sqlServerOptions, sagaSerializer))
            {
                await messageBus.
                    Publish(new SagaBeforeStoredMessage(saga));

                await sagaStore.Store(saga);
            }
        }

    }
}
