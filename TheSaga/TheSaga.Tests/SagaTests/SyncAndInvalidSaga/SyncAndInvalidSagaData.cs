using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Tests.SagaTests.SyncAndInvalidSaga
{
    public class SyncAndInvalidSagaData : ISagaData
    {
        public SyncAndInvalidSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}
