using System;

namespace TheSaga.Providers.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}
