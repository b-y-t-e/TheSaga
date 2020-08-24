using System;
using TheSaga.Interfaces;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Events
{
    public class Dostarczono : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}
