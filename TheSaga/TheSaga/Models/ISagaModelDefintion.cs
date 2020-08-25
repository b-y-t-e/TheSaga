using System;
using TheSaga.SagaStates;

namespace TheSaga.Models
{
    public interface ISagaModelDefintion<TSagaState> where TSagaState : ISagaState
    {
        ISagaModel<TSagaState> GetModel(IServiceProvider serviceProvider);
    }
}