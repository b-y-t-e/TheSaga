using System;
using System.Threading.Tasks;
using TheSaga.Models;

namespace TheSaga.Errors
{
    public interface IErrorHandler
    {
        Task Handle(ISaga saga, Exception error);
    }
}