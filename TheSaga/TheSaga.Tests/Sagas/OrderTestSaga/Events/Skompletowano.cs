using System;
using TheSaga.Interfaces;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Events
{
    public class Skompletowano : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}
