using System;
using System.Collections.Generic;
using TheSaga.SagaStates;

namespace TheSaga.Tests.Sagas.AsyncAndValid
{
    public class AsyncState : ISagaState
    {
        public AsyncState()
        {
        }

        public Guid CorrelationID { get; set; }
        public SagaData SagaState { get; set; }
        public SagaInfo SagaInfo { get; set; }
    }
}