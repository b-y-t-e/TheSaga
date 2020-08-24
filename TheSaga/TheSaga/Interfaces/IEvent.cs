using System;

namespace TheSaga.Interfaces
{
    public interface IEvent
    {
        Guid CorrelationID { get; }
    }

}