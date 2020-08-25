using System;
using TheSaga.Execution;
using TheSaga.Models;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Registrator
{
    public interface ISagaRegistrator
    {
        void Register<TSagaState>(ISagaModel<TSagaState> model)
            where TSagaState : ISagaState;
        
        /*void Register<TSagaState, TSagaModel>(TSagaModel model)
           where TSagaState : ISagaState
           where TSagaModel : ISagaModel<TSagaState>;
        */

       ISagaModel FindModelForEventType(Type eventType);

        ISagaExecutor FindExecutorForStateType(Type stateType);


    }
}