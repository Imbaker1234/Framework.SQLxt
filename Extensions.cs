using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SQLxt
{
    public static class Extensions
    {
        public static List<Dictionary<string, object>> ToResultList(this IDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    results.Add(Enumerable.Range(0, reader.FieldCount)
                        .ToDictionary(reader.GetName, reader.GetValue));
                }
            }

            return results;
        }

        public static Dictionary<string, object> ToKeyValuePairs(this IDataReader reader)
        {
            if (reader.FieldCount != 2)
                throw new Exception("This method can only convert results with exactly two columns.");

            var product = new Dictionary<string, object>();

            while (reader.Read())
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
    }
}