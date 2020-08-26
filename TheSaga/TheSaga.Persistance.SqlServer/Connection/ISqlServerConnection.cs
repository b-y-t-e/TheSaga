using System;
using System.Data;

namespace TheSaga.Persistance.SqlServer.Connection
{
    public interface ISqlServerConnection : IDisposable
    {
        IDbConnection Connection();
    }
}