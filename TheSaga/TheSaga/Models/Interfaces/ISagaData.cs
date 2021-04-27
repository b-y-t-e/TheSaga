using System;

namespace TheSaga.Models.Interfaces
{
    public interface ISagaData
    {
        /// <summary>
        ///     Saga's correlation ID
        /// </summary>
        public Guid ID { get; set; }
    }

    public class EmptySagaData : ISagaData
    {
        public Guid ID { get; set; }
    }
}
