using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

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
    }
}