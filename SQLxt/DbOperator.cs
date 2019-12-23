using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace SQLxt
{
    public class DbOperator : IDbOperator
    {
        private readonly string _connectionString;
        private readonly DatabaseType _type;

        public DbOperator(string connectionString)
        {
            _connectionString = connectionString;
            _type = DetermineType(connectionString);
        }

        public DbOperator(string connectionString, DatabaseType type)
        {
            _connectionString = connectionString;
            _type = type;
        }

        public DbConnection Connection()
        {
            switch (_type)
            {
                case DatabaseType.ODBC:
                    return new OdbcConnection(_connectionString);

                case DatabaseType.OLE:
                    return new OleDbConnection(_connectionString);

                case DatabaseType.SQL:
                    return new SqlConnection(_connectionString);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public DbCommand Command(DbConnection connection, string sql)
        {
            switch (_type)
            {
                case DatabaseType.ODBC:
                    return new OdbcCommand(sql, (OdbcConnection) connection);

                case DatabaseType.OLE:
                    return new OleDbCommand(sql, (OleDbConnection) connection);

                case DatabaseType.SQL:
                    return new SqlCommand(sql, (SqlConnection) connection);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DatabaseType DetermineType(string connectionString)
        {
            if (connectionString.ToLower().Contains("provider") &&
                connectionString.ToLower().Contains("initial catalog"))
            {
                return DatabaseType.OLE;
            }

            if (connectionString.ToLower().Contains("driver"))
            {
                return DatabaseType.ODBC;
            }

            return DatabaseType.SQL;
        }

        public List<Dictionary<string, object>> Select(string sql)
        {
            SanityCheck(sql);

            DbDataReader product;

            List<Dictionary<string, object>> result = null;

            using (var cnn = Connection())
            {
                cnn.Open();
                using (var cmd = Command(cnn, sql))
                {
                    result = cmd.ExecuteReader().ToDictionary();
                }
            }

            return result;
        }

        public int ExecuteNonQuery(string sql)
        {
            SanityCheck(sql);

            int product;

            using (var cnn = Connection())
            {
                cnn.Open();
                using (var cmd = Command(cnn, sql))
                {
                    product = cmd.ExecuteNonQuery();
                }
            }

            return product;
        }


        public List<Dictionary<string, object>> SelectLike(string selectThis, string fromTable, string whereThis, string likeThat,
            bool not = false)
        {
            string sql =
                $"SELECT {selectThis} " +
                $"FROM {fromTable} " +
                $"WHERE {whereThis} ";

            if (not) sql += "NOT ";

            sql += $"LIKE '{likeThat}'";

            List<Dictionary<string, object>> product = null;

            using (var cnn = Connection())
            {
                cnn.Open();
                using (var cmd = Command(cnn, sql))
                {
                    product = cmd.ExecuteReader().ToDictionary();
                }
            }

            return product;
        }

        public object SelectScalar(string selectThis, string fromTable, string whereThis, string equalsThat,
            bool not = false)
        {
            string sql =
                $"SELECT {selectThis} " +
                $"FROM {fromTable} " +
                $"WHERE {whereThis} ";

            if (not) sql += '!';

            sql += $"= '{equalsThat}'";

            object product;

            using (var cnn = Connection())
            {
                cnn.Open();
                using (var cmd = Command(cnn, sql))
                {
                    product = cmd.ExecuteScalar();
                }
            }

            return product;
        }

        public List<Dictionary<string, object>> Select(string selectThis, string fromTable, string whereThis,
            string equalsThat,
            bool not = false)
        {
            string sql =
                $"SELECT {selectThis} " +
                $"FROM {fromTable} " +
                $"WHERE {whereThis} ";

            if (not) sql += '!';

            sql += $"= '{equalsThat}'";

            List<Dictionary<string, object>> product = null;

            using (var cnn = Connection())
            {
                cnn.Open();
                using (var cmd = Command(cnn, sql))
                {
                    product = cmd.ExecuteReader().ToDictionary();
                }
            }

            return product;
        }

        public void SanityCheck(string sql)
        {
            var s = sql.ToLower();
            if ((s.Contains("update") || s.Contains("delete"))
                && !s.Contains("where"))
                throw new Exception("Cannot run a DELETE or UPDATE statement " +
                                    "without a WHERE clause.");

        }
    }
}