using System;

namespace TheSaga
{
    public interface ISaga<TState> : ISaga
        where TState : ISagaState
    {
        void Define<TSagaType>(ISagaBuilder<TSagaType, TState> builder)
            where TSagaType : ISaga<TState>;
    }

    public interface ISaga
    {
    }
}
