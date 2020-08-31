using System;
using TheSaga.Models;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndValid
{
    public class OrderData : ISagaData
    {
        public OrderData()
        {

        }

        public Guid ID { get; set; }
    }
}