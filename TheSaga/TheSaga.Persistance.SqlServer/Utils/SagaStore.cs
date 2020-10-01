using Dapper;
using Dapper.Contrib.Extensions;
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
    public class SagaDb
    {
        public Guid ID { get; set; }
        public String ModelName { get; set; }
        public String State { get; set; }
        public String Step { get; set; }
        public Boolean IsCompensating { get; set; }
        public Boolean IsResuming { get; set; }
        public Boolean IsDeleted { get; set; }
        public Boolean IsBreaked { get; set; }
        public String DataJson { get; set; }
        public String InfoJson { get; set; }
        public String StateJson { get; set; }
        public String ValuesJson { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class SagaStore : IDisposable
    {
        ISqlServerConnection _con;

        IDateTimeProvider _dateTimeProvider;

        SqlServerOptions _sqlServerOptions;

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
                if (ex.Number == 208 || ex.Number == 207)
                    await createStorageTable();
                await saveStorageData(saga);
            }
        }
        private async Task createStorageTable()
        {
            var sql = @$"

if not exists(select 1 from information_schema.tables where table_name = '{_sqlServerOptions.TableName}')
    create table {_sqlServerOptions.TableName} (ID uniqueidentifier not null primary key)

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'ModelName')
    alter table {_sqlServerOptions.TableName} add ModelName nvarchar(1000);

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'State')
    alter table {_sqlServerOptions.TableName} add State nvarchar(1000);

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'Step')
    alter table {_sqlServerOptions.TableName} add Step nvarchar(4000);

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'IsCompensating')
    alter table {_sqlServerOptions.TableName} add IsCompensating bit not null;

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'IsResuming')
    alter table {_sqlServerOptions.TableName} add IsResuming bit not null;

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'IsDeleted')
    alter table {_sqlServerOptions.TableName} add IsDeleted bit not null;

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'IsBreaked')
    alter table {_sqlServerOptions.TableName} add IsBreaked bit not null;

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'DataJson')
    alter table {_sqlServerOptions.TableName} add DataJson nvarchar(max);

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'InfoJson')
    alter table {_sqlServerOptions.TableName} add InfoJson nvarchar(max);

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'StateJson')
    alter table {_sqlServerOptions.TableName} add StateJson nvarchar(max);

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'ValuesJson')
    alter table {_sqlServerOptions.TableName} add ValuesJson nvarchar(max);

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'Created')
    alter table {_sqlServerOptions.TableName} add Created datetime;

if not exists(select 1 from information_schema.columns where table_name = '{_sqlServerOptions.TableName}' and column_name = 'Modified')
    alter table {_sqlServerOptions.TableName} add Modified datetime;
            ";

            _con.Connection().Execute(sql);
        }

        public async Task<ISaga> Get(Guid id)
        {
            try
            {
                return getStorageData(id);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 208 || ex.Number == 207)
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
                if (ex.Number == 208 || ex.Number == 207)
                    await createStorageTable();
                return getUnfinishedStorageData();
            }
        }

        private IList<Guid> getUnfinishedStorageData()
        {
            using var reader = _con.Connection().ExecuteReader(
                $"select ID from {_sqlServerOptions.TableName} where step is not null ");

            List<Guid> guids = new List<Guid>();
            while (reader.Read())
                guids.Add(reader.GetGuid(0));

            return guids;
        }

        private ISaga getStorageData(Guid id)
        {
            SagaDb sagaDb = get(id);
            if (sagaDb == null)
                return null;

            return new Saga()
            {
                Data = JsonConvert.DeserializeObject<ISagaData>(
                    sagaDb.DataJson, _serializerSettings),
                ExecutionState = JsonConvert.DeserializeObject<SagaExecutionState>(
                    sagaDb.StateJson, _serializerSettings),
                ExecutionInfo = JsonConvert.DeserializeObject<SagaExecutionInfo>(
                    sagaDb.InfoJson, _serializerSettings),
                ExecutionValues = JsonConvert.DeserializeObject<SagaExecutionValues>(
                    sagaDb.ValuesJson, _serializerSettings)
            };
        }

        private SagaDb get(Guid id)
        {
            return _con.Connection().QueryFirstOrDefault<SagaDb>(
                $"select * from {_sqlServerOptions.TableName} where ID = @id",
                new { id = id });
        }

        public async Task Remove(Guid id)
        {
            try
            {
                removeStorageData(id);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 208 || ex.Number == 207)
                    await createStorageTable();
                removeStorageData(id);
            }
        }

        private void removeStorageData(Guid id)
        {
            _con.Connection().Execute(
                $"delete from {_sqlServerOptions.TableName} where ID = @id",
                new { id = id });
        }

        private async Task saveStorageData(ISaga saga)
        {
            Boolean exists = true;
            SagaDb sagaDb = get(saga.Data.ID);
            if (sagaDb == null)
            {
                exists = false;
                sagaDb = new SagaDb()
                {
                    ID = saga.Data.ID,
                    Created = _dateTimeProvider.Now,
                    Modified = _dateTimeProvider.Now,
                };
            }


            sagaDb.DataJson = JsonConvert.SerializeObject(
                saga.Data, _serializerSettings);
            sagaDb.InfoJson = JsonConvert.SerializeObject(
                        saga.ExecutionInfo, _serializerSettings);
            sagaDb.StateJson = JsonConvert.SerializeObject(
                        saga.ExecutionState, _serializerSettings);
            sagaDb.ValuesJson = JsonConvert.SerializeObject(
                        saga.ExecutionValues, _serializerSettings);
            sagaDb.IsCompensating = saga.ExecutionState.IsCompensating;
            sagaDb.IsDeleted = saga.ExecutionState.IsDeleted;
            sagaDb.IsResuming = saga.ExecutionState.IsResuming;
            sagaDb.IsBreaked = saga.ExecutionState.IsBreaked;
            sagaDb.ModelName = saga.ExecutionInfo.ModelName;
            sagaDb.State = saga.ExecutionState.CurrentState;
            sagaDb.Step = saga.ExecutionState.CurrentStep;

            if (exists)
            {
                _con.Connection().Save(
                    _sqlServerOptions.TableName, sagaDb,
                    "ID", false);
            }
            else
            {
                _con.Connection().Save(
                    _sqlServerOptions.TableName, sagaDb,
                    "ID", true);
            }
        }

        /*
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
        */

        /*
        async Task<string> generateTableScriptForType()
        {
            StringBuilder script = new StringBuilder();

            if (!(await tableExists(_sqlServerOptions.TableName)))
                script.Append(@$" 
                    if object_id('{_sqlServerOptions.TableName}') is null 
                      create table {_sqlServerOptions.TableName} (
                        {idColumn} uniqueidentifier not null primary key, 
                        {stateNameColumn} nvarchar(500) not null, 
                        {jsonColumn} nvarchar(max), 
                        {createdColumn} datetime, 
                        {modifiedColumn} datetime, 
                        {stateColumn} nvarchar(500), 
                        {stepColumn} nvarchar(500), 
                        {compensatingColumn} int, 
                      ); 
                ");

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
        class columnInfo
        {
            public string dbName;
            public string csName;
        }
        */

        public void Dispose()
        {
            _con.Dispose();
        }

    }
}
