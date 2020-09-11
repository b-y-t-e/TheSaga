using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance.SqlServer.Connection;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Providers;
using TheSaga.Providers.Interfaces;

namespace TheSaga.Persistance.SqlServer.Utils
{
    public class SagaStore : IDisposable
    {
        ISqlServerConnection _con;

        IDateTimeProvider _dateTimeProvider;

        SqlServerOptions _sqlServerOptions;

        string idColumn = "id";

        string stateNameColumn = "name";

        string jsonColumn = "json";

        string createdColumn = "created";

        string modifiedColumn = "modified";

        string stateColumn = "state";

        string stepColumn = "step";

        string compensatingColumn = "iscompensating";

        JsonSerializerSettings _serializerSettings;

        public SagaStore(ISqlServerConnection con, IDateTimeProvider dateTimeProvider, SqlServerOptions sqlServerOptions)
        {
            _con = con;
            _dateTimeProvider = dateTimeProvider;
            _sqlServerOptions = sqlServerOptions;
            _serializerSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
        }

        public async Task Store(ISaga saga)
        {
            if (saga == null)
                return;

            try
            {
                await saveStorageData(saga);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 208)
                    await createStorageTable();
                await saveStorageData(saga);
            }
        }
        private async Task createStorageTable()
        {
            _con.Connection().Execute(await generateTableScriptForType());
        }


        public async Task<ISaga> Get(Guid id)
        {
            try
            {
                return getStorageData(id);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 208)
                    await createStorageTable();
                return getStorageData(id);
            }
        }

        public async Task<IList<Guid>> GetUnfinished()
        {
            try
            {
                return getUnfinishedStorageData();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 208)
                    await createStorageTable();
                return getUnfinishedStorageData();
            }
        }

        private IList<Guid> getUnfinishedStorageData()
        {
            using var reader = _con.Connection().ExecuteReader(
                $"select {idColumn} from {_sqlServerOptions.TableName} where step is not null ");

            List<Guid> guids = new List<Guid>();
            while (reader.Read())
                guids.Add(reader.GetGuid(0));

            return guids;
        }

        private ISaga getStorageData(Guid id)
        {
            string json = _con.Connection().ExecuteScalar<string>(
                $"select {jsonColumn} from {_sqlServerOptions.TableName} where {idColumn} = @id",
                new { id = id });

            if (json == null)
                return null;

            object stateObject = JsonConvert.DeserializeObject(json, _serializerSettings);
            return (ISaga)stateObject;
        }

        public async Task Remove(Guid id)
        {
            try
            {
                removeStorageData(id);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 208)
                    await createStorageTable();
                removeStorageData(id);
            }
        }

        private void removeStorageData(Guid id)
        {
            _con.Connection().Execute(
                $"delete from {_sqlServerOptions.TableName} where {idColumn} = @id",
                new { id = id });
        }

        private async Task saveStorageData(ISaga saga)
        {
            StringBuilder sqlScript = new StringBuilder();
            if (await stateExists(saga.Data.ID))
                sqlScript.Append(generateUpdateScriptForObject(saga));
            else
                sqlScript.Append(generateInsertScriptForObject(saga));

            Dictionary<string, object> dbobjectObject = prepareDbObject(saga);

            if (sqlScript.Length <= 0)
                return;

            _con.Connection().Execute(sqlScript.ToString(), dbobjectObject);
        }

        private Dictionary<string, object> prepareDbObject(ISaga saga)
        {
            Type sagaDataType = saga.Data.GetType();

            Dictionary<string, object> dbobject = new Dictionary<string, object>();
            dbobject[idColumn] = saga.Data.ID;
            dbobject[stateNameColumn] = sagaDataType.Name;
            dbobject[createdColumn] = saga.ExecutionInfo.Created;
            dbobject[modifiedColumn] = saga.ExecutionInfo.Modified;
            dbobject[stateColumn] = saga.ExecutionState.CurrentState;
            dbobject[stepColumn] = saga.ExecutionState.CurrentStep;
            dbobject[compensatingColumn] = saga.ExecutionState.IsCompensating;
            dbobject[jsonColumn] = JsonConvert.SerializeObject(saga, _serializerSettings);

            return dbobject;
        }

        async Task<bool> stateExists(Guid id)
        {
            try
            {
                return (await _con.Connection().ExecuteScalarAsync<int?>(
                $"select 1 from {_sqlServerOptions.TableName} where {idColumn} = @id",
                new { id = id })) != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string generateInsertScriptForObject(ISaga @state)
        {
            Type type = @state.GetType();
            StringBuilder script = new StringBuilder();

            script.Append($"insert into {_sqlServerOptions.TableName} ({idColumn},{stateNameColumn},{jsonColumn},{createdColumn},{modifiedColumn},{stateColumn},{stepColumn},{compensatingColumn}");

            script.Append($") select @{idColumn},@{stateNameColumn},@{jsonColumn},@{createdColumn},@{modifiedColumn},@{stateColumn},@{stepColumn},@{compensatingColumn}");

            script.Append($"; ");

            return script.ToString();
        }

        private string generateUpdateScriptForObject(ISaga @state)
        {
            Type type = @state.GetType();
            StringBuilder script = new StringBuilder();

            script.Append($" update {_sqlServerOptions.TableName} set ");

            script.Append($" {stateNameColumn} = @{stateNameColumn}, ");
            script.Append($" {jsonColumn} = @{jsonColumn}, ");
            script.Append($" {createdColumn} = @{createdColumn}, ");
            script.Append($" {modifiedColumn} = @{modifiedColumn}, ");
            script.Append($" {stateColumn} = @{stateColumn}, ");
            script.Append($" {stepColumn} = @{stepColumn}, ");
            script.Append($" {compensatingColumn} = @{compensatingColumn} ");

            script.Append($" where {idColumn} = @{idColumn}; ");

            return script.ToString();
        }

        async Task<string> generateTableScriptForType()
        {
            StringBuilder script = new StringBuilder();

            if (!(await tableExists(_sqlServerOptions.TableName)))
                script.
                    Append($" if object_id('{_sqlServerOptions.TableName}') is null").
                    Append($" create table {_sqlServerOptions.TableName} (").
                    Append($"   {idColumn} uniqueidentifier not null primary key, ").
                    Append($"   {stateNameColumn} nvarchar(500) not null, ").
                    Append($"   {jsonColumn} nvarchar(max), ").
                    Append($"   {createdColumn} datetime, ").
                    Append($"   {modifiedColumn} datetime, ").
                    Append($"   {stateColumn} nvarchar(500), ").
                    Append($"   {stepColumn} nvarchar(500), ").
                    Append($"   {compensatingColumn} int, ").
                    Append($" ); ");

            return script.ToString();
        }

        private async Task<bool> columnExists(string tablename, string columnName)
        {
            bool exists = (await _con.Connection().ExecuteScalarAsync<int?>(
                $"select 1 from information_schema.columns where table_name = @tablename and column_name = @columnName",
                new { tablename = tablename, columnName = columnName })) > 0;
            return exists;
        }

        private async Task<bool> tableExists(string tablename)
        {
            bool exists = (await _con.Connection().ExecuteScalarAsync<int?>(
                $"select 1 from information_schema.tables where table_name = @tablename",
                new { tablename = tablename })) > 0;
            return exists;
        }

        public void Dispose()
        {
            _con.Dispose();
        }

        class columnInfo
        {
            public string dbName;
            public string csName;
        }
    }
}
