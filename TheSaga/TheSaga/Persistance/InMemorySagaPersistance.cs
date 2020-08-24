using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.States;

namespace TheSaga.Persistance
{
    public class InMemorySagaPersistance : ISagaPersistance
    {
        Dictionary<Guid, string> instances;

        public InMemorySagaPersistance()
        {
            this.instances = new Dictionary<Guid, string>();
        }

        public async Task<ISagaState> Get(Guid correlationID)
        {
            string instance = null;
            instances.TryGetValue(correlationID, out instance);
            return (ISagaState)JsonConvert.DeserializeObject(instance, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        }

        public async Task Set(ISagaState sagaState)
        {
            instances[sagaState.CorrelationID] = JsonConvert.SerializeObject(sagaState, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        }
    }
}