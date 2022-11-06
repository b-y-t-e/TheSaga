using System;

namespace TheSaga.Persistance.Sqlite.Options
{
    public class SqliteOptions
    {
        public SqliteOptions() // string connectionString, string tableName = null)
        {
            //ConnectionString = connectionString;
            //TableName = tableName ?? "TheSagaState";
            TableName = "TheSaga";
        }

        public String ConnectionString { get; set; }

        public String TableName { get; set; }
    }
}