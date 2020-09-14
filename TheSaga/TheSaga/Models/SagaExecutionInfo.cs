using System;

namespace TheSaga.Models
{
    public class SagaExecutionInfo : ISagaExecutionInfo
    {
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string ModelName { get; set; }
    }
}