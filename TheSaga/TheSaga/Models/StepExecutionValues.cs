using System;
using System.Collections.Generic;

namespace TheSaga.Models
{
    public class StepExecutionValues : Dictionary<String, Object>, IStepExecutionValues
    {
        public object Get(string name)
        {
            object val = null;
            this.TryGetValue(name, out val);
            return val;
        }

        internal void Set(IDictionary<string, object> executionValues)
        {
            this.Clear();
            if (executionValues != null)
                foreach (var item in executionValues)
                    this[item.Key] = item.Value;
        }
    }
}