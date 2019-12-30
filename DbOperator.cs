using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SQLxt
{
    public abstract class DbOperator : IDbOperator
    {
        public string ConnectionString { get; set; }

        protected DbOperator(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public abstract IDbConnection GetConnection(string connectionString = null);

        public abstract IDbCommand Command(string sql, IDbConnection connection);

        //CORE
        public List<Dictionary<string, object>> Query(string sql)
        {
            SanityCheck(sql);

            List<Dictionary<string, object>> result = null;

            using (var cnn = GetConnection())
            {
                cnn.Open();
                using (var cmd = Command(sql, cnn))
                {
                    result = cmd.ExecuteReader().ToResultList();
                }
            }

            return result;
        }

        //CORE
        public int ExecuteNonQuery(string sql)
        {
            SanityCheck(sql);

            int product;
            using (var cnn = GetConnection())
            {
                cnn.Open();
                using (var cmd = Command(sql, cnn))
                {
                    product = cmd.ExecuteNonQuery();
                }
            }

            return product;
        }

        public List<Dictionary<string, object>> SelectLike(string selectThis, string fromTable, string whereThis, string likeThat, bool not = false)
        {
            var sql = new StringBuilder();
            sql.Append(
                $"SELECT {selectThis} " +
                $"FROM {fromTable} " +
                $"WHERE {whereThis} ");

            if (not) sql.Append("NOT ");

            sql.Append($"LIKE '{likeThat}'");

            return Query(sql.ToString());
        }

        //Keep
        public int Update(string updateTable, string setThis, string toThat, string whereThis, bool not = false, params string[] inThat)
        {
            var sql = new StringBuilder();
            sql.Append(
                $"UPDATE {updateTable} " +
                $"SET {setThis} = {toThat} " + 
                $"WHERE {whereThis}");

            if (not) sql.Append("NOT ");

            sql.Append(inThat.ToCsv());

            return ExecuteNonQuery(sql.ToString());
        }

        //Keep
        public object SelectScalar(string selectThis, string fromTable, string whereThis, string equalsThat)
        {
            string sql =
                $"SELECT {selectThis} " +
                $"FROM {fromTable} " +
                $"WHERE {whereThis} " +
                $"= '{equalsThat}'";

            object product;

            using (var cnn = GetConnection())
            {
                cnn.Open();
                using (var cmd = Command(sql, cnn))
                {
                    product = cmd.ExecuteScalar();
                }
            }

            return product;
        }

        //Keep
        public List<Dictionary<string, object>> Select(string selectThis, string fromTable, string whereThis,
            bool not = false, params string[] inThat)
        {
            var sql = new StringBuilder();
            sql.Append(
                $"SELECT {selectThis} " +
                $"FROM {fromTable} " +
                $"WHERE {whereThis} ");

            if (not) sql.Append("NOT ");

            sql.Append(inThat.ToCsv());

            return Query(sql.ToString());
        }


        public int Delete(string deleteThis, string fromTable, string whereThis, bool not = false,
            params string[] inThat)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(
                $"DELETE FROM {fromTable} " +
                $"WHERE {whereThis} ");

            if (not) sql.Append("NOT ");

            sql.Append(inThat.ToCsv());

            return ExecuteNonQuery(sql.ToString());
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