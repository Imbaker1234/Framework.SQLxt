using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace SQLxt
{
    internal class OleDbOperator : DbOperator
    {
        public OleDbOperator(string connectionString) : base(connectionString)
        {
        }

        public override IDbConnection GetConnection(string connectionString = null)
        {
            if (connectionString is null)
                connectionString = ConnectionString 
                                   ?? throw new NullReferenceException(
                                       "A connection string was not " +
                                       "set at the DbOperator level nor provided as a parameter.");

            return new OleDbConnection(connectionString);
        }

        public override IDbCommand Command(string sql, IDbConnection connection)
        {
            return new OleDbCommand(sql, (OleDbConnection) connection);
        }
    }
}