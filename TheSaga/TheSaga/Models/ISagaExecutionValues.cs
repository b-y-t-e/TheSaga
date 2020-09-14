using System;
using System.Collections.Generic;

namespace TheSaga.Models
{
    public interface ISagaExecutionValues : IDictionary<String, Object>
    {
        object Get(string name);
    }
}