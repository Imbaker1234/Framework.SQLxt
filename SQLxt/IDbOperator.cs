using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace SQLxt
{
    public interface IDbOperator
    {
        /// <summary>
        /// Returns a DbConnection object of the appropriate type
        /// for the database in use.
        /// </summary>
        /// <returns></returns>
        DbConnection Connection();

        /// <summary>
        /// Provides a DbCommand object of the appropriate type
        /// for the database in use.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        DbCommand Command(DbConnection connection, string sql);

        /// <summary>
        /// Determines what type of database is specified by the
        /// connection string used to create this service.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>


        /// <summary>
        /// Provides an unguided approach to executing a query against
        /// the database, returning the resulting records.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> Select(string sql);

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql);

        /// <summary>
        /// <para>
        /// Provides a guided approach to generating a query in which the
        /// column 'whereThis' matches the pattern specified in 'likeThat'.
        /// </para>
        /// <para>
        /// EXAMPLE:
        /// </para>
        /// <para>
        /// DbService.SelectLike("LastName", "Candidates", "LastName", "%man"); 
        /// </para>
        /// <para>
        /// The 'not' parameter causes the inverse of the query to be returned.
        /// </para>
        /// <para>
        /// GENERATES:
        /// </para>
        /// <para>
        /// "SELECT LastName FROM Candidates WHERE LastName LIKE '%man");
        /// </para>
        /// </summary>
        /// <param name="selectThis"></param>
        /// <param name="fromTable"></param>
        /// <param name="whereThis"></param>
        /// <param name="likeThat"></param>
        /// <param name="not"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> SelectLike(string selectThis, string fromTable, string whereThis, string likeThat, bool not);

        /// <summary>
        /// <para>
        /// Provides a guided approach to creating a query, then retrieves
        /// the first column the first record garnered by that query.
        /// </para>
        /// <para>
        /// The 'not' parameter causes the inverse of the query to be returned.
        /// </para>
        /// <para>
        /// EXAMPLE:
        /// </para>
        /// <para>
        /// DbService.SelectScalar("Name", "Customers", "Address", "555 Holiday Street");
        /// </para>
        /// <para>
        /// GENERATES:
        /// </para>
        /// <para>
        /// "SELECT Name FROM Customers WHERE Address = '555 Holiday Street'"
        /// </para>
        /// </summary>
        /// <param name="selectThis"></param>
        /// <param name="fromTable"></param>
        /// <param name="whereThis"></param>
        /// <param name="equalsThat"></param>
        /// <param name="not"></param>
        /// <returns></returns>
        object SelectScalar(string selectThis, string fromTable, string whereThis, string equalsThat, bool not = false);

        /// <summary>
        /// <para>
        /// Provides a guided approach to selecting records from a specific table
        /// which match a particular ID.
        /// </para>
        /// <para>
        /// The 'not' parameter causes the inverse of the query to be returned.
        /// </para>
        /// <para>
        /// EXAMPLE:
        /// </para>
        /// <para>
        /// DbService.Select("Discount", "Items", "ItemName", "Ugly Christmas Sweater");
        /// </para>
        /// <para>
        /// GENERATES:
        /// </para>
        /// <para>
        /// "SELECT Discount FROM Items WHERE ItemName = 'UglyChristmasSweater'"
        /// </para>
        /// </summary>
        /// <param name="selectThis"></param>
        /// <param name="fromTable"></param>
        /// <param name="whereThis"></param>
        /// <param name="equalsThat"></param>
        /// <param name="not"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> Select(string selectThis, string fromTable, string whereThis,
            string equalsThat, bool not = false);

        /// <summary>
        /// <para>
        /// Provides a safeguard when using the non-structured single parameter
        /// queries to protect against dangerous operations such as a DELETE
        /// without a WHERE clause.
        /// </para>
        /// </summary>
        /// <param name="sql"></param>
        void SanityCheck(string sql);
    }
}