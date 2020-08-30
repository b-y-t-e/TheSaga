using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheSaga.Models;

namespace TheSaga.Errors
{
    public class ErrorHandler : IErrorHandler
    {
        public Task Handle(ISaga saga, Exception error)
        {
            return Task.CompletedTask;
        }
    }
}
