namespace TheSaga.Models.Context
{
    public class ExecutionContext<TSagaData> : IExecutionContext<TSagaData>
        where TSagaData : ISagaData
    {
        public ExecutionContext(TSagaData data, SagaInfo info, SagaState state)
        {
            Data = data;
            Info = info;
            State = state;
        }

        public TSagaData Data { get; set; }

        public SagaInfo Info { get; set; }

        public SagaState State { get; set; }
    }
}