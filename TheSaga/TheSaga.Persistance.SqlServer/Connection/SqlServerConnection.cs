using System.Data;
using System.Data.SqlClient;

namespace TheSaga.Persistance.SqlServer.Connection
{
    public class SqlServerConnection : ISqlServerConnection
    {
        private readonly string connectionString;

        private SqlConnection sqlConnection;

        public SqlServerConnection(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection Connection()
        {
            if (sqlConnection == null)
            {
                sqlConnection = new SqlConnection(connectionString);
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
            ((SqlConnection)sender).Disposed -= SqlConnection_Disposed;
            sqlConnection = null;
        }
    }
}