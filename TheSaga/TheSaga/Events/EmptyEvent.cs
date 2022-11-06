using System;

namespace TheSaga.Events
{
    public class EmptyEvent : ISagaEvent
    {
        /// <summary>
        ///     Saga's correlation ID
        /// </summary>
        public Guid ID { get => Guid.Empty; set { } }
    }
}