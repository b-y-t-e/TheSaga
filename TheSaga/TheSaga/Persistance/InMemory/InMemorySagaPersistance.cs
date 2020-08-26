using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.SagaStates;

namespace TheSaga.Persistance.InMemory
{
    public class InMemorySagaPersistance : ISagaPersistance
    {
        private Dictionary<Guid, string> instances;

        public InMemorySagaPersistance()
        {
            this.instances = new Dictionary<Guid, string>();
        }

        public async Task<ISagaData> Get(Guid correlationID)
        {
            string instance = null;
            instances.TryGetValue(correlationID, out instance);
            if (instance == null)
                return null;
            return (ISagaData)JsonConvert.DeserializeObject(instance, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        }

        public Task Remove(Guid correlationID)
        {
            instances.Remove(correlationID);
            return Task.CompletedTask;
        }

        public async Task Set(ISagaData sagaData)
        {
            instances[sagaData.CorrelationID] = JsonConvert.SerializeObject(sagaData, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        }
    }
}