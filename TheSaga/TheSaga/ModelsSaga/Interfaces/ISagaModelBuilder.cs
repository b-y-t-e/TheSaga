using TheSaga.Models.Interfaces;

namespace TheSaga.ModelsSaga.Interfaces
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
