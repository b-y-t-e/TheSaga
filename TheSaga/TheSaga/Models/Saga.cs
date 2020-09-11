using TheSaga.Models.Interfaces;

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

        public SagaExecutionInfo ExecutionInfo
        {
            get
            {
                if (info == null)
                    info = new SagaExecutionInfo();
                return info;
            }
            set { info = value; }
        }

        public SagaExecutionState ExecutionState
        {
            get
            {
                if (state == null)
                    state = new SagaExecutionState();
                return state;
            }
            set { state = value; }
        }

        public SagaExecutionValues ExecutionValues
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
