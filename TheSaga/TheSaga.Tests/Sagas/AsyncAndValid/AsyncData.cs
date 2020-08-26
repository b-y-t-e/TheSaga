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
        public SagaState SagaState { get; set; }
        public SagaInfo SagaInfo { get; set; }
    }
}