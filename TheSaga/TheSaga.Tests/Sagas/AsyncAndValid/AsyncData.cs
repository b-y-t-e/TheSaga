using System;
using System.Collections.Generic;
using TheSaga.SagaStates;

namespace TheSaga.Tests.Sagas.AsyncAndValid
{
    public class AsyncData : ISagaData
    {
        public AsyncData()
        {
        }

        public Guid CorrelationID { get; set; }
    }
}