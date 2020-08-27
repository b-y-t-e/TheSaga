using System;

namespace TheSaga.Events
{
    public interface IEvent
    {
        /// <summary>
        /// Saga's correlation ID
        /// </summary>
        Guid ID { get; }
    }
}