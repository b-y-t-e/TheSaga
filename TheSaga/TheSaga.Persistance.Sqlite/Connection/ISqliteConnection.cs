using System;
using System.Data;

namespace TheSaga.Persistance.Sqlite.Connection
{
    public interface ISqliteConnection : IDisposable
    {
        IDbConnection Connection();
    }
}