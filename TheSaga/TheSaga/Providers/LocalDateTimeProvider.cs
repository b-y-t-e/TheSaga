using System;
using TheSaga.Providers.Interfaces;

namespace TheSaga.Providers
{
    public class LocalDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
