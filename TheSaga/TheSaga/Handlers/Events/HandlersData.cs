using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Handlers.Events
{
    public class HandlersData : ISagaData
    {
        public Guid ID { get; set; }
    }
}
