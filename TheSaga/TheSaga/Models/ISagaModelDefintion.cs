using System;
using TheSaga.SagaStates;

namespace TheSaga.Models
{
    public interface ISagaModelDefintion<TSagaData> where TSagaData : ISagaData
    {
        ISagaModel<TSagaData> GetModel(IServiceProvider serviceProvider);
    }
}