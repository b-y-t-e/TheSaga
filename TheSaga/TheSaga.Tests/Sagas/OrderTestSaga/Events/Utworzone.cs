using System;
using TheSaga.Interfaces;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Events
{
    public class Utworzone : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}
