using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance.SqlServer.Connection;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Providers;
using TheSaga.Providers.Interfaces;
using TheSaga.Serializer;

namespace TheSaga.Persistance.SqlServer.Utils
{
    public class SagaDb
    {
        public long _ID { get; set; }
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

        ISagaSerializer _sagaSerializer;

        static bool _isInitalized;

        public SagaStore(ISqlServerConnection con, IDateTimeProvider dateTimeProvider, SqlServerOptions sqlServerOptions, ISagaSerializer sagaSerializer)
        {
            _con = con;
            _dateTimeProvider = dateTimeProvider;
            _sqlServerOptions = sqlServerOptions;
            _sagaSerializer = sagaSerializer;

            if (!_isInitalized)
            {
                createStorageTable().GetAwaiter().GetResult();
                _isInitalized = true;
            }
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
    create table {getTableName()} (_ID int primary key identity(1,1))

if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = 'ID')
    alter table {getTableName()} add ID uniqueidentifier not null;

declare @pkname varchar(max), @sql varchar(max)
if not exists(select 1 from information_schema.columns where table_name = '{getTableName()}' and column_name = '_ID')
begin
    set @pkname = (select name from sys.key_constraints where parent_object_id = object_id('{getTableName()}'))
    if @pkname is not null
	begin
	    set @sql = 'ALTER TABLE {getTableName()} DROP CONSTRAINT '+@pkname
		exec(@sql)
	end    
	set @sql = 'ALTER TABLE {getTableName()} alter column ID uniqueidentifier not null'
	exec(@sql)
	set @sql = 'ALTER TABLE {getTableName()} add _ID int primary key identity(1,1)'
	exec(@sql)
end

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

if not exists(select 1 from sys.indexes where name = 'ind_{getTableName()}_id')
    create index ind_{getTableName()}_id on {getTableName()}(id);
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
                $" select ID " +
                $" from {getTableName()} " +
                $" where " +
                $"   step is not null " +
                $"   and IsDeleted = 0 " +
                $" order by " +
                $"   (case when parentid is not null and state = '' then 1 else 0 end) desc ");

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

            if (_sqlServerOptions.Compression)
            {
                try
                {
                    return DecompressAndDeserializeSaga(sagaDb);
                }
                catch (Exception ex)
                {
                    try
                    {
                        return DeserializeSaga(sagaDb);
                    }
                    catch
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                return DeserializeSaga(sagaDb);
            }
        }

        private ISaga DecompressAndDeserializeSaga(SagaDb sagaDb)
        {
            return new Saga()
            {
                Data = _sagaSerializer.Deserialize<ISagaData>(ZipHelper.Unzip(sagaDb.DataJson)),
                ExecutionState = _sagaSerializer.Deserialize<SagaExecutionState>(ZipHelper.Unzip(sagaDb.StateJson)),
                ExecutionInfo = _sagaSerializer.Deserialize<SagaExecutionInfo>(ZipHelper.Unzip(sagaDb.InfoJson)),
                ExecutionValues = _sagaSerializer.Deserialize<SagaExecutionValues>(ZipHelper.Unzip(sagaDb.ValuesJson))
            };
        }

        private ISaga DeserializeSaga(SagaDb sagaDb)
        {
            return new Saga()
            {
                Data = _sagaSerializer.Deserialize<ISagaData>(sagaDb.DataJson),
                ExecutionState = _sagaSerializer.Deserialize<SagaExecutionState>(sagaDb.StateJson),
                ExecutionInfo = _sagaSerializer.Deserialize<SagaExecutionInfo>(sagaDb.InfoJson),
                ExecutionValues = _sagaSerializer.Deserialize<SagaExecutionValues>(sagaDb.ValuesJson)
            };
        }

        private SagaDb get(Guid id)
        {
            return _con.Connection().QueryFirstOrDefault<SagaDb>(
                $" select * " +
                $" from {getTableName()} " +
                $" where " +
                $"   IsDeleted = 0 " +
                $"   and ID = @id",
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
                $" delete from {getTableName()} " +
                $" where ID = @id",
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

            if (_sqlServerOptions.Compression)
            {
                sagaDb.DataJson = ZipHelper.ZipToString(_sagaSerializer.Serialize(saga.Data));
                sagaDb.InfoJson = ZipHelper.ZipToString(_sagaSerializer.Serialize(saga.ExecutionInfo));
                sagaDb.StateJson = ZipHelper.ZipToString(_sagaSerializer.Serialize(saga.ExecutionState));
                sagaDb.ValuesJson = ZipHelper.ZipToString(_sagaSerializer.Serialize(saga.ExecutionValues));
            }
            else
            {
                sagaDb.DataJson = _sagaSerializer.Serialize(saga.Data);
                sagaDb.InfoJson = _sagaSerializer.Serialize(saga.ExecutionInfo);
                sagaDb.StateJson = _sagaSerializer.Serialize(saga.ExecutionState);
                sagaDb.ValuesJson = _sagaSerializer.Serialize(saga.ExecutionValues);
            }

            sagaDb.Modified = _dateTimeProvider.Now;
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
                await _con.Connection().ExecuteAsync(
                    $" update {getTableName()} set " +
                    $"   ModelName = @ModelName, " +
                    $"   Name = @Name, " +
                    $"   State = @State, " +
                    $"   Step = @Step, " +
                    $"   IsCompensating = @IsCompensating, " +
                    $"   IsResuming = @IsResuming, " +
                    $"   IsDeleted = @IsDeleted, " +
                    $"   IsBreaked = @IsBreaked, " +
                    $"   DataJson = @DataJson, " +
                    $"   InfoJson = @InfoJson, " +
                    $"   StateJson = @StateJson, " +
                    $"   ValuesJson = @ValuesJson, " +
                    $"   Modified = @Modified, " +
                    $"   CanBeResumed = @CanBeResumed, " +
                    $"   ParentId = @ParentId " +
                    $" where " +
                    $"   _ID = {sagaDb._ID}",
                    sagaDb);
            }
            else
            {
                sagaDb._ID = await _con.Connection().ExecuteScalarAsync<long>(
                    $" insert into {getTableName()} (ID, ModelName, Name, State, Step, IsCompensating, IsResuming, IsDeleted, IsBreaked, DataJson, InfoJson, StateJson, ValuesJson, Created, Modified, CanBeResumed, ParentId) " +
                    $" select @ID, @ModelName, @Name, @State, @Step, @IsCompensating, @IsResuming, @IsDeleted, @IsBreaked, @DataJson, @InfoJson, @StateJson, @ValuesJson, @Created, @Modified, @CanBeResumed, @ParentId; " +
                    $" select SCOPE_IDENTITY(); ",
                    sagaDb);
            }
        }

        string getTableName() =>
            TemplateHelper.CorrectTemplateName(_sqlServerOptions.TableName);

        public void Dispose()
        {
            _con.Dispose();
        }
    }
}
