using System;
using TheSaga.Interfaces;

namespace TheSaga.Tests.Sagas.OrderTestSaga
{
    public class Utworzone : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}
