using System;

namespace TheSaga.Events
{
    internal class EmptyEvent : IEvent
    {
        public Guid CorrelationID
        {
            get => Guid.Empty;
        }
    }
}