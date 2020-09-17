using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Persistance.SqlServer
{
    internal class WeakInMemorySagaPersistance 
    {
        private readonly TimeSpan timeInMemory;
        private Dictionary<Guid, InMemorySnapshot> instances;

        public WeakInMemorySagaPersistance(TimeSpan timeInMemory)
        {
            instances = new Dictionary<Guid, InMemorySnapshot>();
            this.timeInMemory = timeInMemory;
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

                return (ISaga)JsonConvert.DeserializeObject(item.Json,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
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
                    Json = JsonConvert.SerializeObject(saga,
                        new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }),
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
