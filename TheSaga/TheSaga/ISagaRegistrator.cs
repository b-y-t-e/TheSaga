using System;
using TheSaga.Model;

namespace TheSaga
{
    public interface ISagaRegistrator
    {
        void Register<TSagaState>(string sagaName, SagaModel<TSagaState> model)
            where TSagaState : ISagaState;
    }
}