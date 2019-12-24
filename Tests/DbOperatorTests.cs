using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SQLxt;

namespace Tests
{
    [TestFixture]
    public class DbOperatorTests
    {
        public string ConnectionString =
            @"Data Source=(local)\SQLEXPRESS;Initial Catalog=SampleDB;Integrated Security=True";

        public string Odbc =
            @"Driver={SQL Server};Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;";

        public string Ole =
            @"Provider=sqloledb;Data Source=myServerAddress;Initial Catalog=myDataBase;User Id=myUsername;Password=myPassword;";

        public void ConnectionTest()
        {
            var sut = new DbOperator(ConnectionString);
            var odbc = new DbOperator(Odbc);
            var ole = new DbOperator(Ole);

            sut.Connection().Should().BeOfType<SqlConnection>();
            ole.Connection().Should().BeOfType<OleDbConnection>();
            odbc.Connection().Should().BeOfType<OdbcConnection>();
        }

        [Test]
        public void CommandTest()
        {
            var sut = new DbOperator(ConnectionString);
            var odbc = new DbOperator(Odbc);
            var ole = new DbOperator(Ole);

            sut.Command("").Should()
                .BeOfType<SqlCommand>("An SQL Operator should provide an SQL connection");
            ole.Command("").Should()
                .BeOfType<OleDbCommand>("An OLE Operator should provide an OLE connection");
            odbc.Command("").Should()
                .BeOfType<OdbcCommand>("An ODBC Operator should provide an ODBC connection");
        }

        [Test]
        public void DeterminedTypeWillBeSQL()
        {
            DbOperator.DetermineType(ConnectionString).Should().Be(DatabaseType.SQL);
            DbOperator.DetermineType(Odbc).Should().Be(DatabaseType.ODBC);
            DbOperator.DetermineType(Odbc).Should().Be(DatabaseType.ODBC);
        }

        [Test]
        public void SelectTest()
        {
            var sut = new DbOperator(ConnectionString);

            string sql = "SELECT TOP 3 * FROM CCS WHERE ID LIKE 'Brown%'";

            var blah = sut.Select(sql);

            var counter = 0;

            foreach (var dict in blah)
            {
                counter++;
                Console.WriteLine($"Record {counter}:");
                foreach (var kvp in dict)
                {
                    Console.WriteLine($"{kvp.Key} - {kvp.Value}");
                }
            }
        }

        [Test]
        public void ExecuteNonQuery()
        {
            var sut = new DbOperator(ConnectionString);

            var one = new Tuple<string, string>("GoldFox", "Saint Louis");
            var two = new Tuple<string, string>("MangeCat", "New York");
            var three = new Tuple<string, string>("KingKong", "Rotunda");

            string insert =
                "INSERT INTO CCS " +
                "VALUES " +
                $"('{one.Item1}', '{one.Item2}'), " +
                $"('{two.Item1}', '{two.Item2}'), " +
                $"('{three.Item1}', '{three.Item2}')";

            sut.ExecuteNonQuery(insert).Should().Be(3);

            sut.SelectScalar("Data", "CCS", "ID", one.Item1).Should().Be(one.Item2);
            sut.SelectScalar("Data", "CCS", "ID", two.Item1).Should().Be(two.Item2);
            sut.SelectScalar("Data", "CCS", "ID", three.Item1).Should().Be(three.Item2);

            string delete = $"DELETE FROM CCS WHERE ID IN('{one.Item1}', '{two.Item1}', '{three.Item1}')";

            sut.ExecuteNonQuery(delete).Should().Be(3);
        }

        [Test]
        public void SelectLikeTest()
        {
            var sut = new DbOperator(ConnectionString);

            var results = sut.SelectLike("DATA", "CCS", "ID", "BROWN%");

            results.Count.Should().Be(3);

            int counter = 0;
            foreach (var result in results)
            {
                counter++;
                Console.WriteLine($"Record {counter}");
                foreach (KeyValuePair<string, object> keyValuePair in result)
                {
                    Console.WriteLine($"     {keyValuePair.Key} - {keyValuePair.Value}\n");
                }
            }
        }

        [Test]
        public void SelectNotLikeTest()
        {
            var sut = new DbOperator(ConnectionString);

            var results = sut.SelectLike("DATA", "CCS", "ID", "BROWN%", true);

            results.Count.Should().NotBe(3);

            int counter = 0;
            foreach (var result in results)
            {
                counter++;
                Console.WriteLine($"Record {counter}");
                foreach (KeyValuePair<string, object> keyValuePair in result)
                {
                    Console.WriteLine($"     {keyValuePair.Key} - {keyValuePair.Value}\n");
                }
            }
        }

        [Test]
        public void ObjectsWillGoOutOfScope()
        {
            var sut = new DbOperator(ConnectionString);

            DbCommand cmd;
            Console.WriteLine("Before the block.");
            using (var cnn = sut.Connection())
            {
                cnn.Open();

                using (cmd = sut.Command("", cnn))
                {
                    Console.WriteLine(cmd.Connection is null ? "Null" : "Present");
                    Console.WriteLine(cmd.Connection is null ? "Null" : "Present");
                    Console.WriteLine(cmd.Connection is null ? "Null" : "Present");
                }
            }

            Console.WriteLine("After the block");
        }

        [Test]
        public void SelectScalarTest()
        {
            var sut = new DbOperator(ConnectionString);

            var result = sut.SelectScalar("Data", "CCS", "ID", "CustomerName").Should().Be("Ray Palmer");
        }

        [Test]
        public void SanityCheckTest()
        {
            var sut = new DbOperator(ConnectionString);

            string goodSql = "DELETE FROM CCS WHERE ID = 'KingKrab'";
            Action good = () => sut.SanityCheck(goodSql);

            string badSql = "DELETE FROM CCS";
            Action bad = () => sut.SanityCheck(badSql);

            string irrelevantSql = "SELECT * FROM CCS";
            Action irrelevant = () => sut.SanityCheck(irrelevantSql);

            good.Should().NotThrow("This has a where clause and is safe.");
            bad.Should().Throw<Exception>("This is missing the WHERE clause and is not safe.");
            irrelevant.Should().NotThrow("This only retrieves records and does not modify the database.");
        }
    }
}