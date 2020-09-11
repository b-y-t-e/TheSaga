using System;
using TheSaga.Events;
using TheSaga.Handlers.Events;

namespace TheSaga.Tests.HanderTests
{
    internal class OrderCreated : IHandlersEvent
    {
        public Guid ID => throw new NotImplementedException();
    }
}