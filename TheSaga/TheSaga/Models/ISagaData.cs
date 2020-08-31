using System;

namespace TheSaga.Models
{
    public interface ISagaData
    {
        /// <summary>
        ///     Saga's correlation ID
        /// </summary>
        public Guid ID { get; set; }
    }
}