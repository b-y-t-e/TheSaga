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
using TheSaga.Persistance.Sqlite.Connection;
using TheSaga.Persistance.Sqlite.Options;
using TheSaga.Providers;
using TheSaga.Providers.Interfaces;
using TheSaga.Serializer;

namespace TheSaga.Persistance.Sqlite.Utils
{
    public class SagaDb
    {
        public String ID { get; set; }
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
        public String? ParentId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class SagaStore : IDisposable
    {
        ISqliteConnection _con;

        IDateTimeProvider _dateTimeProvider;

        SqliteOptions _SqliteOptions;

        ISagaSerializer sagaSerializer;

        public SagaStore(ISqliteConnection con, IDateTimeProvider dateTimeProvider, SqliteOptions SqliteOptions, ISagaSerializer sagaSerializer)
        {
            _con = con;
            _dateTimeProvider = dateTimeProvider;
            _SqliteOptions = SqliteOptions;
            this.sagaSerializer = sagaSerializer;
        }

        public async Task Store(ISaga saga)
        {
            if (saga == null)
                return;

            try
            {
                await saveStorageData(saga);
            }
            catch (Exception ex)
            {
                if (CheckDatabaseStructureError(ex))
                    await createStorageTable();
                await saveStorageData(saga);
            }
        }

        private static bool CheckDatabaseStructureError(Exception ex)
        {
            return
                ex.Message.ToLower().Contains("no such table") ||
                ex.Message.ToLower().Contains("no such column") ||
                ex.Message.ToLower().Contains("has no column name");
        }

        private async Task createStorageTable()
        {
            TryExecute($" create table {getTableName()} (ID text not null primary key) ");
            TryExecute($" alter table {getTableName()} add ModelName text ");
            TryExecute($" alter table {getTableName()} add Name text ");
            TryExecute($" alter table {getTableName()} add State text; ");
            TryExecute($" alter table {getTableName()} add Step text ");
            TryExecute($" alter table {getTableName()} add IsCompensating int not null default(0) ");
            TryExecute($" alter table {getTableName()} add IsResuming int not null default(0) ");
            TryExecute($" alter table {getTableName()} add IsDeleted int not null default(0) ");
            TryExecute($" alter table {getTableName()} add IsBreaked int not null default(0) ");
            TryExecute($" alter table {getTableName()} add DataJson text ");
            TryExecute($" alter table {getTableName()} add InfoJson text ");
            TryExecute($" alter table {getTableName()} add StateJson text ");
            TryExecute($" alter table {getTableName()} add ValuesJson text ");
            TryExecute($" alter table {getTableName()} add Created datetime ");
            TryExecute($" alter table {getTableName()} add Modified datetime ");
            TryExecute($" alter table {getTableName()} add CanBeResumed int ");
            TryExecute($" alter table {getTableName()} add ParentId text ");
            TryExecute($"  ");
        }

        private void TryExecute(string sql)
        {
            if ((sql ?? "").Trim() == "")
                return;
            try { _con.Connection().Execute(sql); }
            catch { }
        }

        public async Task<ISaga> Get(Guid id)
        {
            try
            {
                return getStorageData(id);
            }
            catch (Exception ex)
            {
                if (CheckDatabaseStructureError(ex))
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
            catch (Exception ex)
            {
                if (CheckDatabaseStructureError(ex))
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
                guids.Add(Guid.Parse(reader.GetString(0)));

            return guids;
        }

        private ISaga getStorageData(Guid id)
        {
            SagaDb sagaDb = get(id);
            if (sagaDb == null)
                return null;

            return new Saga()
            {
                Data = sagaSerializer.Deserialize<ISagaData>(sagaDb.DataJson),
                ExecutionState = sagaSerializer.Deserialize<SagaExecutionState>(sagaDb.StateJson),
                ExecutionInfo = sagaSerializer.Deserialize<SagaExecutionInfo>(sagaDb.InfoJson),
                ExecutionValues = sagaSerializer.Deserialize<SagaExecutionValues>(sagaDb.ValuesJson)
            };
        }

        private SagaDb get(Guid id)
        {
            return _con.Connection().QueryFirstOrDefault<SagaDb>(
                $"select * from {getTableName()} where ID = @id",
                new { id = id.ToString().ToUpper() });
        }

        public async Task Remove(Guid id)
        {
            try
            {
                removeStorageData(id);
            }
            catch (Exception ex)
            {
                if (CheckDatabaseStructureError(ex))
                    await createStorageTable();
                removeStorageData(id);
            }
        }

        private void removeStorageData(Guid id)
        {
            _con.Connection().Execute(
                $"delete from {getTableName()} where ID = @id",
                new { id = id.ToString().ToUpper() });
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
                    ID = saga.Data.ID.ToString().ToUpper(),
                    Created = _dateTimeProvider.Now,
                    Modified = _dateTimeProvider.Now,
                };
            }

            sagaDb.Modified = _dateTimeProvider.Now;
            sagaDb.DataJson = sagaSerializer.Serialize(saga.Data);
            sagaDb.InfoJson = sagaSerializer.Serialize(saga.ExecutionInfo);
            sagaDb.StateJson = sagaSerializer.Serialize(saga.ExecutionState);
            sagaDb.ValuesJson = sagaSerializer.Serialize(saga.ExecutionValues);
            sagaDb.IsCompensating = saga.ExecutionState.IsCompensating;
            sagaDb.IsDeleted = saga.ExecutionState.IsDeleted;
            sagaDb.IsResuming = saga.ExecutionState.IsResuming;
            sagaDb.IsBreaked = saga.ExecutionState.IsBreaked;
            sagaDb.Name = saga.Data?.GetType().Name ?? "";
            sagaDb.ModelName = saga.ExecutionInfo.ModelName;
            sagaDb.State = saga.ExecutionState.CurrentState;
            sagaDb.Step = saga.ExecutionState.CurrentStep;
            sagaDb.CanBeResumed = saga.ExecutionState.CanBeResumed ? 1 : 0;
            sagaDb.ParentId = saga.ExecutionState.ParentID == null ? null : saga.ExecutionState.ParentID.ToString().ToUpper();

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
            TemplateHelper.CorrectTemplateName(_SqliteOptions.TableName);

        public void Dispose()
        {
            _con.Dispose();
        }

    }
}
