using System;
using TheSaga.Interfaces;

namespace TheSaga.Tests.Sagas.OrderTestSaga
{
    public class Dostarczono : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}
