using System;
using TheSaga.States;

namespace TheSaga.Tests.Sagas.OrderTestSaga
{
    public class OrderState : ISagaState
    {
        public Guid CorrelationID { get; set; }
        public string CurrentState { get; set; }
        public string CurrentStep { get; set; }
        public bool IsCompensating { get; set; }
    }
}
