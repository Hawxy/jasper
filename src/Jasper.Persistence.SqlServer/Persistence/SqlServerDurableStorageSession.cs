using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Jasper.Messaging.Durability;
using Jasper.Messaging.Logging;

namespace Jasper.Persistence.SqlServer.Persistence
{
    public class SqlServerDurableStorageSession : IDurableStorageSession
    {
        private readonly SqlServerSettings _settings;
        private readonly CancellationToken _cancellation;

        public SqlServerDurableStorageSession(SqlServerSettings settings, CancellationToken cancellation)
        {
            _settings = settings;
            _cancellation = cancellation;
        }

        internal SqlTransaction Transaction { get; private set; }

        internal SqlConnection Connection { get; private set; }

        internal SqlCommand CreateCommand(string sql)
        {
            return new SqlCommand(sql, Connection, Transaction);
        }

        internal SqlCommand CallFunction(string functionName)
        {
            var cmd = new SqlCommand(_settings.SchemaName + "." + functionName, Connection, Transaction)
            {
                CommandType = CommandType.StoredProcedure
            };

            return cmd;
        }

        public Task ReleaseNodeLock(int lockId)
        {
            return Connection.ReleaseGlobalLock(lockId, _cancellation);
        }

        public Task GetNodeLock(int lockId)
        {
            return Connection.GetGlobalLock(lockId, _cancellation);
        }

        public Task Begin()
        {
            Transaction = Connection.BeginTransaction();
            return Task.CompletedTask;
        }

        public Task Commit()
        {
            Transaction.Commit();
            Transaction = null;
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
            Transaction.Rollback();
            return Task.CompletedTask;
        }

        public Task<bool> TryGetGlobalTxLock(int lockId)
        {
            return Connection.TryGetGlobalTxLock(Transaction, lockId, _cancellation);
        }

        public Task<bool> TryGetGlobalLock(int lockId)
        {
            return Connection.TryGetGlobalLock(lockId, Transaction, _cancellation);
        }

        public Task ReleaseGlobalLock(int lockId)
        {
            return Connection.ReleaseGlobalLock(lockId, _cancellation, Transaction);
        }

        public bool IsConnected()
        {
            return Connection?.State == ConnectionState.Open;
        }

        public async Task ConnectAndLockCurrentNode(ITransportLogger logger, int nodeId)
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                }
            }

            try
            {
                Connection = new SqlConnection(_settings.ConnectionString);

                // TODO -- use the CancellationToken from JasperSettings
                await Connection.OpenAsync(_cancellation);

                await Connection.GetGlobalLock(nodeId, _cancellation, Transaction);
            }
            catch (Exception e)
            {
                Connection?.Dispose();
                Connection = null;

                throw;
            }
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            Connection?.Dispose();
        }
    }
}
