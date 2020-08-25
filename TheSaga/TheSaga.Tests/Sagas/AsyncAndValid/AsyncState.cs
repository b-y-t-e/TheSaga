using System;
using System.Collections.Generic;
using TheSaga.SagaStates;

namespace TheSaga.Tests.Sagas.AsyncAndValid
{
    public class AsyncState : ISagaState
    {
        public AsyncState()
        {
            History = new List<SagaStepLog>();
        }

        public Guid CorrelationID { get; set; }
        public string CurrentState { get; set; }
        public string CurrentStep { get; set; }
        public bool IsCompensating { get; set; }
        public Exception CurrentError { get; set; }
        public IList<SagaStepLog> History { get; set; }
    }
}