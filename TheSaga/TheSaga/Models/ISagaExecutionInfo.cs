using System;

namespace TheSaga.Models
{
    public interface ISagaExecutionInfo
    {
        DateTime Created { get;  }
        string ModelName { get;  }
        DateTime Modified { get;  }
    }
}