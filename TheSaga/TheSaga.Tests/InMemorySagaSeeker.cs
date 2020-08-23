﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheSaga.Tests
{
    internal class InMemorySagaSeeker : ISagaSeeker
    {
        Dictionary<Guid, ISagaInstance> instances;

        public InMemorySagaSeeker()
        {
            this.instances = new Dictionary<Guid, ISagaInstance>();
        }

        public async Task<ISagaInstance> Seek(Guid correlationID)
        {
            ISagaInstance instance = null;
            instances.TryGetValue(correlationID, out instance);
            return instance;
        }
    }
}