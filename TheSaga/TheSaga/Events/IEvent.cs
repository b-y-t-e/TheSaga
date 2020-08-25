using System;

namespace TheSaga.Events
{
    public interface IEvent
    {
        Guid CorrelationID { get; }
    }
}