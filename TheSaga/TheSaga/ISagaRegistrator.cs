using System;

namespace TheSaga
{
    public interface ISagaRegistrator
    {
        void Register(ISaga saga);
    }
}