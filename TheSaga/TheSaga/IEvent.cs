using System;

namespace TheSaga
{
    public interface IEvent
    {
        Guid CorrelationID { get; }
    }

}