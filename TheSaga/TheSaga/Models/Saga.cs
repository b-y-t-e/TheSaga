namespace TheSaga.Models
{
    public class Saga : ISaga
    {
        public Saga()
        {
            Info = new SagaInfo();
            State = new SagaState();
        }

        public ISagaData Data { get; set; }

        public SagaInfo Info { get; set; }

        public SagaState State { get; set; }
    }
}