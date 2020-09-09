using System;

namespace TheSaga.Providers
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}