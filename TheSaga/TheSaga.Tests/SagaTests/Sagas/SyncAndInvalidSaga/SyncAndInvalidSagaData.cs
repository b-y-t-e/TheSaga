using System;
using TheSaga.Models;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndInvalidSaga
{
    public class SyncAndInvalidSagaData : ISagaData
    {
        public SyncAndInvalidSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}