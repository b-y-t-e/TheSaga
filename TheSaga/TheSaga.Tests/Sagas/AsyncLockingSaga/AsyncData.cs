using System;
using System.Collections.Generic;
using TheSaga.SagaStates;

namespace TheSaga.Tests.Sagas.AsyncLockingSaga
{
    public class AsyncData : ISagaData
    {
        public AsyncData()
        {

        }

        public Guid CorrelationID { get; set; }
    }
}