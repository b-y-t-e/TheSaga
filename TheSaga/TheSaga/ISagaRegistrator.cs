using System;

namespace TheSaga
{
    public interface ISagaRegistrator
    {
        void Register<TSagaState>(string sagaName)
            where TSagaState : ISagaState;
    }
}