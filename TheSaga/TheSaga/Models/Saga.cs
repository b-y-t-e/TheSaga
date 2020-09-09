namespace TheSaga.Models
{
    public class Saga : ISaga
    {
        public Saga()
        {
            Info = new SagaInfo();
            State = new SagaExecutionState();
        }

        public ISagaData Data { get; set; }

        public SagaInfo Info { get; set; }

        public SagaExecutionState State { get; set; }
    }
}