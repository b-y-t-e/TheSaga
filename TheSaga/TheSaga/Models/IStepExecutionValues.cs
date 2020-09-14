using System;
using System.Collections.Generic;

namespace TheSaga.Models
{
    public interface IStepExecutionValues : IDictionary<String, Object>
    {
        object Get(string name);
    }
}