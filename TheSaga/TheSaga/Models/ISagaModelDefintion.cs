using System;
using TheSaga.Models;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Models
{
    public interface ISagaModelDefintion<TSagaState> where TSagaState : ISagaState
    {
        ISagaModel<TSagaState> GetModel(IServiceProvider serviceProvider);
    }
}