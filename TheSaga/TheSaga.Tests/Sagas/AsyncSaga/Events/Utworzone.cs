using System;
using TheSaga.Interfaces;

namespace TheSaga.Tests.Sagas.AsyncSaga.Events
{
    public class Utworzone : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}
