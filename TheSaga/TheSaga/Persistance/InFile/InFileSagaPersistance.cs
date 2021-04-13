using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Messages;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Persistance.InFile
{
    public class InFileSagaPersistance : ISagaPersistance
    {
        private readonly IMessageBus messageBus;

        public InFileSagaPersistance(IMessageBus messageBus)
        {
            this.messageBus = messageBus;
        }

        public async Task<ISaga> Get(Guid id)
        {
            if (!File.Exists($"{id}.json"))
                return null;

            var json = File.
                ReadAllText($"{id}.json");

            var saga = (ISaga)JsonConvert.DeserializeObject(json,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            await messageBus.
                Publish(new SagaAfterRetrivedMessage(saga));

            return saga;
        }

        public async Task<IList<Guid>> GetUnfinished()
        {
            throw new NotImplementedException();
        }

        public async Task Remove(Guid id)
        {
            if (File.Exists($"{id}.json"))
                File.Delete($"{id}.json");
        }

        public async Task Set(ISaga saga)
        {
            if (File.Exists($"{saga.Data.ID}.json"))
                File.Delete($"{saga.Data.ID}.json");

            var json = JsonConvert.SerializeObject(saga,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            await messageBus.
                Publish(new SagaBeforeStoredMessage(saga));

            File.
                WriteAllText($"{saga.Data.ID}.json", json);
        }
    }
}
