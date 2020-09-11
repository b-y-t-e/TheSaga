using System;

namespace TheSaga.Events
{
    public interface ISagaEvent
    {
        /// <summary>
        ///     Saga's correlation ID
        /// </summary>
        Guid ID { get; }
    }
}