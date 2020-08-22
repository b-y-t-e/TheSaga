using System;

namespace TheSaga
{
    public interface ISagaRegistrator
    {
        void Register<TState>(ISaga<TState> saga)
            where TState : ISagaState;
    }
}