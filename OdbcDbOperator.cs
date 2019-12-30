using System;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace SQLxt
{
    internal class OdbcDbOperator : DbOperator
    {
        public OdbcDbOperator(string connectionString) : base(connectionString)
        {
        }

        public override IDbConnection GetConnection(string connectionString = null)
        {
            if (connectionString is null)
                connectionString = ConnectionString 
                                   ?? throw new NullReferenceException(
                                       "A connection string was not set at the DbOperator level nor " +
                                       "provided as a parameter.");

            return new OdbcConnection(connectionString);
        }

        public override IDbCommand Command(string sql, IDbConnection connection)
        {
            return new OdbcCommand(sql, (OdbcConnection) connection);
        }
    }
}