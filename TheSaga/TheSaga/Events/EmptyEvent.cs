using System;

namespace TheSaga.Events
{
    internal class EmptyEvent : IEvent
    {
        /// <summary>
        ///     Saga's correlation ID
        /// </summary>
        public Guid ID => Guid.Empty;
    }
}