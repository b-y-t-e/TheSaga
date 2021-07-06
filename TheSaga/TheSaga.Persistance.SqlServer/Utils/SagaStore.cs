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
        public String Name { get; set; }
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
        public Int32 CanBeResumed { get; set; }
        public Guid? ParentId { get; set; }
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

if not exists(select 1 from information_schema.tables where table_name = '{getTableName()}')
    create table {getTableName()} (ID uniqueidentifier not null primary key)

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'ModelName')
    alter table {getTableName()} add ModelName nvarchar(1000);

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'Name')
    alter table {getTableName()} add Name nvarchar(1000);

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'State')
    alter table {getTableName()} add State nvarchar(1000);

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'Step')
    alter table {getTableName()} add Step nvarchar(4000);

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'IsCompensating')
    alter table {getTableName()} add IsCompensating bit not null default(0);

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'IsResuming')
    alter table {getTableName()} add IsResuming bit not null default(0);

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'IsDeleted')
    alter table {getTableName()} add IsDeleted bit not null default(0);

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'IsBreaked')
    alter table {getTableName()} add IsBreaked bit not null default(0);

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'DataJson')
    alter table {getTableName()} add DataJson nvarchar(max);

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'InfoJson')
    alter table {getTableName()} add InfoJson nvarchar(max);

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'StateJson')
    alter table {getTableName()} add StateJson nvarchar(max);

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'ValuesJson')
    alter table {getTableName()} add ValuesJson nvarchar(max);

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'Created')
    alter table {getTableName()} add Created datetime;

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'Modified')
    alter table {getTableName()} add Modified datetime;

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'CanBeResumed')
    alter table {getTableName()} add CanBeResumed int;

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'ParentId')
    alter table {getTableName()} add ParentId uniqueidentifier;
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
                $" select ID from {getTableName()} where step is not null order by (case when parentid is not null and state = '' then 1 else 0 end) desc ");

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
                $"select * from {getTableName()} where ID = @id",
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
                $"delete from {getTableName()} where ID = @id",
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

            sagaDb.Modified = _dateTimeProvider.Now;
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
            sagaDb.Name = saga.Data?.GetType().Name ?? "";
            sagaDb.ModelName = saga.ExecutionInfo.ModelName;
            sagaDb.State = saga.ExecutionState.CurrentState;
            sagaDb.Step = saga.ExecutionState.CurrentStep;
            sagaDb.CanBeResumed = saga.ExecutionState.CanBeResumed ? 1 : 0;
            sagaDb.ParentId = saga.ExecutionState.ParentID;

            if (exists)
            {
                _con.Connection().Save(
                    getTableName(), sagaDb,
                    "ID", false);
            }
            else
            {
                _con.Connection().Save(
                    getTableName(), sagaDb,
                    "ID", true);
            }
        }

        string getTableName() =>
            CorrectTemplateName(_sqlServerOptions.TableName);
        string CorrectTemplateName(string name)
        {
            name = name.
                Replace(".", "_").Replace("-", "_").Replace(" ", "_").
                Replace("[", "_").Replace("]", "_").
                Replace("__", "_").Replace("__", "_");

            return name;
        }
        public void Dispose()
        {
            _con.Dispose();
        }

    }
}
