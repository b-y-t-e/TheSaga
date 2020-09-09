using TheSaga.Models;

namespace TheSaga.SagaModels
{
    public interface ISagaModelBuilder<TSagaData>
        where TSagaData : ISagaData
    {
        ISagaModel Build();
    }

    /* public interface ISagaModelBuilder
     {
         ISagaModel Build();
     }*/
}