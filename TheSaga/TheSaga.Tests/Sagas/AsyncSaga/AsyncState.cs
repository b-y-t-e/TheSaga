using System;
using System.Collections.Generic;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Tests.Sagas.AsyncSaga
{
    public class AsyncState : ISagaState
    {
        public Guid CorrelationID { get; set; }
        public string CurrentState { get; set; }
        public string CurrentStep { get; set; }
        public bool IsCompensating { get; set; }
        public List<String> Logs { get; set; }
        public AsyncState()
        {
            Logs = new List<string>();
        }
    }
}
