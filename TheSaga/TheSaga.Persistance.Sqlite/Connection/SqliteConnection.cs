using System.Data;
using System.Data.SqlClient;

namespace TheSaga.Persistance.Sqlite.Connection
{
    public class SqliteConnection : ISqliteConnection
    {
        private readonly string connectionString;

        private Microsoft.Data.Sqlite.SqliteConnection sqlConnection;

        public SqliteConnection(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection Connection()
        {
            if (sqlConnection == null)
            {
                sqlConnection = new(connectionString);
                sqlConnection.Disposed += SqlConnection_Disposed;
                sqlConnection.Open();
            }
            return sqlConnection;
        }

        public void Dispose()
        {
            if (sqlConnection != null)
            {
                sqlConnection.Dispose();
                sqlConnection = null;
            }
        }

        private void SqlConnection_Disposed(object sender, System.EventArgs e)
        {
            ((Microsoft.Data.Sqlite.SqliteConnection)sender).Disposed -= SqlConnection_Disposed;
            sqlConnection = null;
        }
    }
}