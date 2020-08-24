using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.States;

namespace TheSaga.Persistance
{
    public class InMemorySagaPersistance : ISagaPersistance
    {
        Dictionary<Guid, ISagaState> instances;

        public InMemorySagaPersistance()
        {
            this.instances = new Dictionary<Guid, ISagaState>();
        }

        public async Task<ISagaState> Get(Guid correlationID)
        {
            ISagaState instance = null;
            instances.TryGetValue(correlationID, out instance);
            return instance;
        }

        public async Task Set(ISagaState sagaState)
        {
            instances[sagaState.CorrelationID] = sagaState;
        }
    }
}