using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SQLxt
{
    public static class Extensions
    {
        public static List<Dictionary<string, object>> ToDictionary(this DbDataReader reader)
        {
            return (from object record in reader
                select Enumerable.Range(0, reader.FieldCount)
                    .ToDictionary(reader.GetName, reader.GetValue)).ToList();
        }

        public static Dictionary<string, object> ToKeyValuePairs(this DbDataReader reader)
        {
            if(reader.FieldCount != 2) throw new Exception("This method can only convert results with exactly two columns.");

            var product = new Dictionary<string, object>();

            while(reader.Read())
            {
                product.Add(reader.GetString(0), reader.GetValue(0));
            }

            return product;
        }

        public static DatabaseType DetermineType(this DbOperator dbOperator, string connectionString)
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

        public static string ToCsv(this string[] strings)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < strings.Length; i++)
            {
                sb.Append($"'{strings[i]}'");
                if (i < strings.Length) sb.Append(", ");
            }

            return sb.ToString();
        }

        public static object ExecuteScalar(this DbCommand command)
        {
            var product = command.ExecuteScalar();
            command.Dispose();

            return product;
        }
    }
}