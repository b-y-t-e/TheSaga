using System;

namespace TheSaga
{
    public interface ISaga<TState> 
        where TState : ISagaState
    {
    }
}
