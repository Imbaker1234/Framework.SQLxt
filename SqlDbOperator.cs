using System;
using System.Data;
using System.Data.SqlClient;

namespace SQLxt
{
    internal class SqlDbOperator : DbOperator
    {
        public SqlDbOperator(string connectionString) : base(connectionString)
        {
        }

        public override IDbConnection GetConnection(string connectionString = null)
        {
            if (connectionString is null)
                connectionString = ConnectionString 
                    ?? throw new NullReferenceException(
                        "A connection string was not set at the DbOperator level nor " +
                                                                 "provided as a parameter.");

            return new SqlConnection(connectionString);
        }

        public override IDbCommand Command(string sql, IDbConnection connection)
        {
            return new SqlCommand(sql, (SqlConnection) connection);
        }
    }
}