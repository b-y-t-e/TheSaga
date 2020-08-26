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

        public async Task<ISagaState> Get(Guid correlationID)
        {
            string instance = null;
            instances.TryGetValue(correlationID, out instance);
            if (instance == null)
                return null;
            return (ISagaState)JsonConvert.DeserializeObject(instance, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        }

        public Task Remove(Guid correlationID)
        {
            instances.Remove(correlationID);
            return Task.CompletedTask;
        }

        public async Task Set(ISagaState sagaState)
        {
            instances[sagaState.CorrelationID] = JsonConvert.SerializeObject(sagaState, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        }
    }
}