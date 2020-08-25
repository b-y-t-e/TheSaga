using System;
using System.Collections.Generic;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Tests.Sagas.OrderTestSaga
{
    public class OrderState : ISagaState
    {
        public Guid CorrelationID { get; set; }
        public string CurrentState { get; set; }
        public string CurrentStep { get; set; }
        public bool IsCompensating { get; set; }
        public List<String> Logs { get; set; }
        public OrderState()
        {
            Logs = new List<string>();
        }
    }
}
