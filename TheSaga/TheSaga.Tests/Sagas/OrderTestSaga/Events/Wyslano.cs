using System;
using TheSaga.Interfaces;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Events
{
    public class Wyslano : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}
