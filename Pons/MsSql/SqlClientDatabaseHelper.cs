using System.Data;
using System.Data.SqlClient;

namespace Pons
{
    public class SqlClientDatabaseHelper
    {
        public static void CreateDatabase( string connectionString, bool createNew)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            string catalog = builder.InitialCatalog;
            builder.InitialCatalog = string.Empty;
            string defaultConnectionString = builder.ToString();
            bool databaseExists = DatabaseExists(defaultConnectionString, catalog);
            if (createNew && databaseExists)
            {
                DatabaseDrop(defaultConnectionString, catalog);
                databaseExists = false;
            }
            if (!databaseExists)
            {
                DatabaseCreate(defaultConnectionString, catalog);
            }
        }

        private static bool DatabaseExists(string connectionString, string databaseName)
        {
            string cmdText = "select 1 from sys.databases where name='%DATABASE_NAME%'"
                .Replace("%DATABASE_NAME%", databaseName);
            object result = Execute(connectionString, cmdText);
            return result != null;
        }

        private static void DatabaseCreate(string connectionString, string databaseName)
        {
            string cmdText = @"CREATE DATABASE [%DATABASE_NAME%]"
                .Replace("%DATABASE_NAME%", databaseName);
            Execute(connectionString, cmdText);
        }

        private static void DatabaseDrop(string connectionString, string databaseName)
        {
            // (re-)create database(s)
            string script =
                @"IF  EXISTS (SELECT [name] FROM sys.databases WHERE [name] = N'%DATABASE_NAME%')
BEGIN
    ALTER DATABASE [%DATABASE_NAME%]
        SET SINGLE_USER
        WITH ROLLBACK IMMEDIATE

    DROP DATABASE [%DATABASE_NAME%]
END
";
            script = script.Replace("%DATABASE_NAME%", databaseName);
            Execute(connectionString, script);
        }

        private static object Execute(string connectionString, string sqlCommand)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            using(conn)
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlCommand;
                return cmd.ExecuteScalar();
            }
        }
    }
}