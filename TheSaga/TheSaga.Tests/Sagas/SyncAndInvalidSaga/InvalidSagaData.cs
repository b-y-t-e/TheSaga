using System;
using System.Collections.Generic;
using TheSaga.SagaStates;

namespace TheSaga.Tests.Sagas.SyncAndInvalidSaga
{
    public class InvalidSagaData : ISagaData
    {
        public InvalidSagaData()
        {

        }

        public Guid CorrelationID { get; set; }
    }
}