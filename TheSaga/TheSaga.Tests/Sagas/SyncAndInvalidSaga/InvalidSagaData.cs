using System;
using System.Collections.Generic;
using TheSaga.Models;

namespace TheSaga.Tests.Sagas.SyncAndInvalidSaga
{
    public class InvalidSagaData : ISagaData
    {
        public InvalidSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}