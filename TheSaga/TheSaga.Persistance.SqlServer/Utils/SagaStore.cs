using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheSaga.Persistance.SqlServer.Connection;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Providers;
using TheSaga.SagaStates;

namespace TheSaga.Persistance.SqlServer.Utils
{
    public class SagaStore : IDisposable
    {
        ISqlServerConnection _con;

        IDateTimeProvider _dateTimeProvider;

        SqlServerOptions _sqlServerOptions;

        string correlationIdColumn = "id";

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
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                // DateTimeZoneHandling = DateTimeZoneHandling.Local
            };
        }

        public async Task Store(ISagaState @state)
        {
            if (@state == null)
                return;

            StringBuilder sqlScript = new StringBuilder();
            if (await stateExists(state.CorrelationID))
                sqlScript.Append(generateUpdateScriptForObject(@state));
            else
                sqlScript.Append(generateInsertScriptForObject(@state));

            var dbobjectObject = prepareDbObject(@state);

            if (sqlScript.Length <= 0)
                return;

            _con.Connection().Execute(sqlScript.ToString(), dbobjectObject);

            /*
            try
            {
                await _con.Get().ExecuteAsync(insertScript.ToString(), dbobjectObject);
            }
            catch
            {
                StringBuilder createScript = new StringBuilder();
                createScript.Append(await generateTableScriptForType(@state.GetType()));
                if (createScript.Length > 0)
                    await _con.Get().ExecuteAsync(createScript.ToString());

                await _con.Get().ExecuteAsync(insertScript.ToString(), dbobjectObject);
            }*/
        }

        public async Task<ISagaState> Get(Guid correlationId)
        {
            try
            {
                string json = _con.Connection().ExecuteScalar<string>(
                    $"select {jsonColumn} from {_sqlServerOptions.TableName} where {correlationIdColumn} = @correlationId",
                    new { correlationId = correlationId });

                object stateObject = JsonConvert.DeserializeObject(json, _serializerSettings);
                return (ISagaState)stateObject;
            }
            catch (Exception ex)
            {
                var e = ex;
                e = e;
                return null;
            }
        }

        public async Task Remove(Guid correlationId)
        {
            _con.Connection().Execute(
                $"delete from {_sqlServerOptions.TableName} where {correlationIdColumn} = @correlationId",
                new { correlationId = correlationId });
        }

        private Dictionary<string, object> prepareDbObject(ISagaState @state)
        {
            Type stateType = @state.GetType();

            Dictionary<string, object> dbobject = new Dictionary<string, object>();
            dbobject[correlationIdColumn] = @state.CorrelationID;
            dbobject[stateNameColumn] = stateType.Name;
            dbobject[createdColumn] = @state.SagaCreated;
            dbobject[modifiedColumn] = @state.SagaModified;
            dbobject[stateColumn] = @state.SagaCurrentState;
            dbobject[stepColumn] = @state.SagaCurrentStep;
            dbobject[compensatingColumn] = @state.SagaIsCompensating;
            dbobject[jsonColumn] = JsonConvert.SerializeObject(@state, _serializerSettings);

            /*foreach (var columnInfo in getColumnsForType(stateType))
            {
                object val = GetValueFromCsPath(@state, columnInfo.csName);
                val = FormatCsValue(val);
                dbobject[columnInfo.dbName] = val;
            }*/

            return dbobject;
        }

        /*private string FormatCsValue(object val)
        {
            if (val != null && val.GetType().IsClass && val.GetType() != typeof(String))
            {
                return JsonConvert.SerializeObject(@val, _serializerSettings);
            }
            else
            {
                return Convert.ToString(val, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        private object GetValueFromCsPath(object obj, string path)
        {
            var type = obj.GetType();
            var propertyNames = path.Split('.');
            var val = obj;
            foreach (var propertyName in propertyNames)
            {
                PropertyInfo? property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                    return null;

                val = property.GetValue(val);
                if (val == null)
                    return null;

                type = val.GetType();
            }
            return val;
        }*/

        async Task<bool> stateExists(Guid correlationId)
        {
            return (await _con.Connection().ExecuteScalarAsync<int?>(
                $"select 1 from {_sqlServerOptions.TableName} where {correlationIdColumn} = @correlationId",
                new { correlationId = correlationId })) != null;
        }

        private string generateInsertScriptForObject(ISagaState @state)
        {
            Type type = @state.GetType();
            StringBuilder script = new StringBuilder();

            script.Append($"insert into {_sqlServerOptions.TableName} ({correlationIdColumn},{stateNameColumn},{jsonColumn},{createdColumn},{modifiedColumn},{stateColumn},{stepColumn},{compensatingColumn}");

            /*foreach (var columnInfo in getColumnsForType(type))
                script.Append($",{columnInfo.dbName}");*/

            script.Append($") select @{correlationIdColumn},@{stateNameColumn},@{jsonColumn},@{createdColumn},@{modifiedColumn},@{stateColumn},@{stepColumn},@{compensatingColumn}");

            /*foreach (var columnInfo in getColumnsForType(type))
                script.Append($",@{columnInfo.dbName}");*/

            script.Append($"; ");

            return script.ToString();
        }

        private string generateUpdateScriptForObject(ISagaState @state)
        {
            Type type = @state.GetType();
            StringBuilder script = new StringBuilder();

            script.Append($" update {_sqlServerOptions.TableName} set ");

            /*foreach (var columnInfo in getColumnsForType(type))
                script.Append($",{columnInfo.dbName}");*/

            script.Append($" {stateNameColumn} = @{stateNameColumn}, ");
            script.Append($" {jsonColumn} = @{jsonColumn}, ");
            script.Append($" {createdColumn} = @{createdColumn}, ");
            script.Append($" {modifiedColumn} = @{modifiedColumn}, ");
            script.Append($" {stateColumn} = @{stateColumn}, ");
            script.Append($" {stepColumn} = @{stepColumn}, ");
            script.Append($" {compensatingColumn} = @{compensatingColumn} ");

            /*foreach (var columnInfo in getColumnsForType(type))
                script.Append($",@{columnInfo.dbName}");*/

            script.Append($" where {correlationIdColumn} = @{correlationIdColumn}; ");

            return script.ToString();
        }

        async Task<string> generateTableScriptForType(Type type)
        {
            StringBuilder script = new StringBuilder();

            if (!(await tableExists(_sqlServerOptions.TableName)))
                script.
                    Append($" create table {_sqlServerOptions.TableName} (").
                    Append($"   {correlationIdColumn} uniqueidentifier not null primary key, ").
                    Append($"   {stateNameColumn} nvarchar(500) not null, ").
                    Append($"   {jsonColumn} nvarchar(max), ").
                    Append($"   {createdColumn} datetime, ").
                    Append($"   {modifiedColumn} datetime, ").
                    Append($"   {stateColumn} nvarchar(500), ").
                    Append($"   {stepColumn} nvarchar(500), ").
                    Append($"   {compensatingColumn} int, ").
                    Append($" ); ");

            /*foreach (columnInfo columnInfo in getColumnsForType(type))
                if (!(await columnExists(_sqlServerOptions.TableName, columnInfo.dbName)))
                    script.Append($"alter table {_sqlServerOptions.TableName} add {columnInfo.dbName} varchar(max) null;");
            */

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

        /*IEnumerable<columnInfo> getColumnsForType(Type type, String parentDbName = "")
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in props)
            {
                string dbName = property.Name;

                if (property.PropertyType != typeof(string) &&
                    property.PropertyType.IsClass)
                {
                    if (string.IsNullOrEmpty(parentDbName))
                        foreach (var columnInfo in getColumnsForType(property.PropertyType, $"{dbName}."))
                            yield return columnInfo;
                    else
                        foreach (var columnInfo in getColumnsForType(property.PropertyType, $"{parentDbName}{dbName}."))
                            yield return columnInfo;
                }
                else
                {
                    if (parentDbName == "")
                    {
                        if (dbName.ToLower() == correlationIdColumn)
                            dbName = "stateId";
                        else if (dbName.ToLower() == stateNameColumn)
                            dbName = "stateName";
                        else if (dbName.ToLower() == jsonColumn)
                            dbName = "stateJson";
                    }

                    yield return new columnInfo()
                    {
                        csName = $"{parentDbName}{property.Name}",
                        dbName = $"{parentDbName}{dbName}".Replace('.', '_')
                    };
                }
            }
        }*/

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
