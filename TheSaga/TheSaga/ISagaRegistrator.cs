using System;

namespace TheSaga
{
    public interface ISagaRegistrator
    {
        void Register<TSagaType, TSataState>(string sagaName)
            where TSagaType : ISaga<TSataState>
            where TSataState : ISagaState;
    }
}