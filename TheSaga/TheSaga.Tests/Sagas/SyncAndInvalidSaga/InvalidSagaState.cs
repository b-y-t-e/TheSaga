using System;
using System.Collections.Generic;
using TheSaga.SagaStates;

namespace TheSaga.Tests.Sagas.SyncAndInvalidSaga
{
    public class InvalidSagaState : ISagaState
    {
        public InvalidSagaState()
        {

        }

        public Guid CorrelationID { get; set; }
        public SagaData SagaState { get; set; }
        public SagaInfo SagaInfo { get; set; }
    }
}