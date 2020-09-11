using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Tests.SagaTests.SyncAndValid
{
    public class OrderData : ISagaData
    {
        public OrderData()
        {

        }

        public Guid ID { get; set; }
    }
}
