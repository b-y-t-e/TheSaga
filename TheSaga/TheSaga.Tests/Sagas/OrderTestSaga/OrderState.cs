using System;
using System.Collections.Generic;
using TheSaga.SagaStates;

namespace TheSaga.Tests.Sagas.OrderTestSaga
{
    public class OrderState : ISagaState
    {
        public OrderState()
        {
            Logs = new List<string>();
        }

        public Guid CorrelationID { get; set; }
        public string CurrentState { get; set; }
        public string CurrentStep { get; set; }
        public bool IsCompensating { get; set; }
        public List<String> Logs { get; set; }
        public string CurrentError { get; set; }
    }
}