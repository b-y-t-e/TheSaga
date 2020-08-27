using System;
using System.Collections.Generic;

namespace TheSaga.SagaStates
{
    public interface ISagaData
    {
        /// <summary>
        /// Saga's correlation ID
        /// </summary>
        public Guid ID { get; set; }
    }
}