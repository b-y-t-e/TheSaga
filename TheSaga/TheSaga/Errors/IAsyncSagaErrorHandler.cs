using System;
using System.Threading.Tasks;
using TheSaga.Models;

namespace TheSaga.Errors
{
    public interface IAsyncSagaErrorHandler
    {
        Task Handle(ISaga saga, Exception error);
    }
}