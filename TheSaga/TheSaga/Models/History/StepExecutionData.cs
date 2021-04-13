using System;

namespace TheSaga.Models.History
{
    public class StepExecutionData
    {
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? SucceedTime { get; set; }
        public DateTime? FailTime { get; set; }
        public Exception Error { get; set; }
        public bool? ConditionResult { get; set; }
    }
}
