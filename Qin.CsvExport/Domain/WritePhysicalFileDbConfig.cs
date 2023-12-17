
namespace Qin.CsvRelevant
{
    using System;
    using System.Data;
    using System.Data.Common;

    public class WritePhysicalFileDbConfig
    {
        public WritePhysicalFileDbConfig(DbConnection conn, string sql)
        {
            this.DbConnection = conn;
            this.Sql = sql;
        }

        public WritePhysicalFileDbConfig(DbConnection dbConnection, string sql, object? param, IDbTransaction? transaction, int? commandTimeout, CommandType? commandType)
           : this(dbConnection, sql)
        {
            Param = param;
            Transaction = transaction;
            CommandTimeout = commandTimeout;
            CommandType = commandType;
        }

        public WritePhysicalFileDbConfig()
        {
        }

        public DbConnection DbConnection { get; set; }

        public string Sql { get; set; }

        public object? Param { get; set; } = null;

        public IDbTransaction? Transaction { get; set; } = null;

        public int? CommandTimeout { get; set; } = null;

        public CommandType? CommandType { get; set; } = null;

        public void Check()
        {
            if (DbConnection == null)
            {
                throw new ArgumentNullException("DbConnection 不能为null", innerException: null);
            }

            if (string.IsNullOrWhiteSpace(Sql))
            {
                throw new ArgumentNullException("Sql 不能为null", innerException: null);
            }
        }
    }
}
