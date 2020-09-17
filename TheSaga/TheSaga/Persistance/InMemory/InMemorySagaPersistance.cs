using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Persistance.InMemory
{
    public class InMemorySagaPersistance : ISagaPersistance
    {
        private Dictionary<Guid, ISaga> objectInstances;
        private Dictionary<Guid, string> serializedInstances;

        public InMemorySagaPersistance()
        {
            serializedInstances = new Dictionary<Guid, string>();
            objectInstances = new Dictionary<Guid, ISaga>();
        }

        public async Task<ISaga> Get(Guid id)
        {
            lock (serializedInstances)
            {
                string json = null;
                serializedInstances.TryGetValue(id, out json);
                if (json == null)
                    return null;

                return (ISaga)JsonConvert.DeserializeObject(json,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            }
        }

        public async Task<IList<Guid>> GetUnfinished()
        {
            lock (serializedInstances)
            {
                IList<ISaga> unfinished = objectInstances.Values.Where(v => !v.IsIdle()).ToArray();
                return unfinished.Select(i => i.Data.ID).ToArray();
            }
        }

        public Task Remove(Guid id)
        {
            lock (serializedInstances)
            {
                serializedInstances.Remove(id);
                return Task.CompletedTask;
            }
        }

        public async Task Set(ISaga saga)
        {
            lock (serializedInstances)
            {
                serializedInstances[saga.Data.ID] = JsonConvert.SerializeObject(saga,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                objectInstances[saga.Data.ID] = saga;
            }
        }
    }
}
