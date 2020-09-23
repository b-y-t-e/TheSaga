using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Persistance.InFile
{
    public class InFileSagaPersistance : ISagaPersistance
    {

        public InFileSagaPersistance()
        {
        }

        public async Task<ISaga> Get(Guid id)
        {
            if (!File.Exists($"{id}.json"))
                return null;

            var json = File.
                ReadAllText($"{id}.json");

            return (ISaga)JsonConvert.DeserializeObject(json,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
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

            File.
                WriteAllText($"{saga.Data.ID}.json", json);
        }
    }
}
