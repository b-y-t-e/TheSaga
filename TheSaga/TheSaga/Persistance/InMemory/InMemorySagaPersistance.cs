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
        private readonly Dictionary<Guid, ISaga> objectInstances;
        private readonly Dictionary<Guid, string> serializedInstances;

        public InMemorySagaPersistance()
        {
            serializedInstances = new Dictionary<Guid, string>();
            objectInstances = new Dictionary<Guid, ISaga>();
        }

        public async Task<ISaga> Get(Guid id)
        {
            string instance = null;
            serializedInstances.TryGetValue(id, out instance);
            if (instance == null)
                return null;

            return (ISaga) JsonConvert.DeserializeObject(instance,
                new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
        }

        public async Task<IList<Guid>> GetUnfinished()
        {
            IList<ISaga> unfinished = objectInstances.Values.Where(v => !v.IsIdle()).ToArray();

            return unfinished.Select(i => i.Data.ID).ToArray();
        }

        public Task Remove(Guid id)
        {
            serializedInstances.Remove(id);
            return Task.CompletedTask;
        }

        public async Task Set(ISaga sagaData)
        {
            serializedInstances[sagaData.Data.ID] = JsonConvert.SerializeObject(sagaData,
                new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
            objectInstances[sagaData.Data.ID] = sagaData;
        }
    }
}
