using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Messages;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Serializer;
using TheSaga.States;

namespace TheSaga.Persistance.InMemory
{
    public class InMemorySagaPersistance : ISagaPersistance
    {
        private readonly IMessageBus messageBus;
        private Dictionary<Guid, ISaga> objectInstances;
        private Dictionary<Guid, string> serializedInstances;
        private readonly ISagaSerializer sagaSerializer;

        public InMemorySagaPersistance(IMessageBus messageBus, ISagaSerializer sagaSerializer)
        {
            serializedInstances = new Dictionary<Guid, string>();
            objectInstances = new Dictionary<Guid, ISaga>();
            this.messageBus = messageBus;
            this.sagaSerializer = sagaSerializer;
        }

        public async Task<ISaga> Get(Guid id)
        {
            ISaga saga = null;

            lock (serializedInstances)
            {
                string json = null;
                serializedInstances.TryGetValue(id, out json);
                if (json == null)
                    return null;

                saga = (ISaga)sagaSerializer.Deserialize(json);
            }

            await messageBus.
                Publish(new SagaAfterRetrivedMessage(saga));

            return saga;
        }

        public async Task<IList<Guid>> GetUnfinished()
        {
            lock (serializedInstances)
            {
                IList<ISaga> unfinished = objectInstances.Values.Where(v => !v.IsIdle()).ToArray();
                return unfinished.
                    OrderByDescending(i => i.ExecutionState.ParentID != null && i.ExecutionState.CurrentState == new SagaStartState().GetStateName() ? 1 : 0).
                    Select(i => i.Data.ID).
                    ToArray();
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
                serializedInstances[saga.Data.ID] = sagaSerializer.Serialize(saga);

                objectInstances[saga.Data.ID] = saga;
            }

            await messageBus.
                Publish(new SagaBeforeStoredMessage(saga));
        }
    }
}
