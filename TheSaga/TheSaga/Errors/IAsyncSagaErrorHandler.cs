using System;
using System.Threading.Tasks;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Errors
{
    public interface IAsyncSagaErrorHandler
    {
        Task Handle(ISaga saga, Exception error);
    }
}
