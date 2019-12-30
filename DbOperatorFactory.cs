using System;

namespace SQLxt
{
    public static class DbOperatorFactory
    {
        private static DatabaseType DetermineType(string connectionString)
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

        public static IDbOperator GetOperator(string connectionString)
        {
            switch (DetermineType(connectionString))
            {
                case DatabaseType.OLE:
                    return new OleDbOperator(connectionString);

                case DatabaseType.ODBC:
                    return new OdbcDbOperator(connectionString);

                case DatabaseType.SQL:
                    return new SqlDbOperator(connectionString);

                default:
                    throw new Exception("Could not determine DbOperator Type from provided connection string.");
            }
        }
    }
}