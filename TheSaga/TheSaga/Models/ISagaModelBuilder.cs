using System;
using TheSaga.Builders;
using TheSaga.SagaStates;

namespace TheSaga.Models
{
    public interface ISagaModelBuilder<TSagaData> 
        where TSagaData : ISagaData
    {
         ISagaModel<TSagaData> Build();
    }

   /* public interface ISagaModelBuilder
    {
        ISagaModel Build();
    }*/
}