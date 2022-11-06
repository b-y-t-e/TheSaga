using System;

namespace TheSaga.Persistance.SqlServer.Options
{
    public class SqlServerOptions
    {
        public SqlServerOptions() // string connectionString, string tableName = null)
        {
            //ConnectionString = connectionString;
            //TableName = tableName ?? "TheSagaState";
            TableName = "TheSaga";
        }

        public String ConnectionString { get; set; }

        public String TableName { get; set; }

        public Boolean Compression { get; set; }
    }
}