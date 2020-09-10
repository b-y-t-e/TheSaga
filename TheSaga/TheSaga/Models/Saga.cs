namespace TheSaga.Models
{
    public class Saga : ISaga
    {
        private SagaExecutionInfo info;
        private SagaExecutionState state;
        private SagaExecutionValues values;

        public Saga()
        {
        }

        public ISagaData Data { get; set; }

        public SagaExecutionInfo Info
        {
            get
            {
                if (info == null)
                    info = new SagaExecutionInfo();
                return info;
            }
            set { info = value; }
        }

        public SagaExecutionState State
        {
            get
            {
                if (state == null)
                    state = new SagaExecutionState();
                return state;
            }
            set { state = value; }
        }

        public SagaExecutionValues Values
        {
            get
            {
                if (values == null)
                    values = new SagaExecutionValues();
                return values;
            }
            set { values = value; }
        }
    }
}