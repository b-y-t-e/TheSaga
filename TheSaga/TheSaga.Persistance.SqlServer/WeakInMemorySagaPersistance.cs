using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Serializer;

namespace TheSaga.Persistance.SqlServer
{
    internal class WeakInMemorySagaPersistance 
    {
        private readonly TimeSpan timeInMemory;
        private Dictionary<Guid, InMemorySnapshot> instances;
        private readonly ISagaSerializer sagaSerializer;

        public WeakInMemorySagaPersistance(TimeSpan timeInMemory, ISagaSerializer sagaSerializer)
        {
            instances = new Dictionary<Guid, InMemorySnapshot>();
            this.timeInMemory = timeInMemory;
            this.sagaSerializer = sagaSerializer;
        }

        public async Task<ISaga> Get(Guid id)
        {
            lock (instances)
            {
                InMemorySnapshot item = null;
                instances.TryGetValue(id, out item);

                if (item == null)
                    return null;

                if (item.IsDead())
                {
                    item.Clean();
                    instances.Remove(id);
                    return null;
                }

                return (ISaga)sagaSerializer.Deserialize(item.Json);
            }
        }

        public Task Remove(Guid id)
        {
            lock (instances)
            {
                instances.Remove(id);
                return Task.CompletedTask;
            }
        }

        public async Task Set(ISaga saga)
        {
            lock (instances)
            {
                instances[saga.Data.ID] = new InMemorySnapshot()
                {
                    Json = sagaSerializer.Serialize(saga),
                    Saga = saga,
                    TTL = DateTime.UtcNow.Add(timeInMemory)
                };
            }
        }
    }

    internal class InMemorySnapshot
    {
        public ISaga Saga;
        public String Json;
        public DateTime TTL;
        public bool IsDead() =>
            DateTime.UtcNow > TTL;
        public void Clean()
        {
            Json = null;
            Saga = null;
        }
    }
}
