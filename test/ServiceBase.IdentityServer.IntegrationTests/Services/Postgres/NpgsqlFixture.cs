using Dapper;
using Npgsql;
using System;
using System.IO;
using Xunit;

namespace ServiceBase.IdentityServer.IntegrationTests
{
    [CollectionDefinition("NpgsqlFixture")]
    public class DatabaseCollection : ICollectionFixture<NpgsqlFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    internal static class NpgsqlConnectionExtensions
    {
        public static int ExecuteFromFile(this NpgsqlConnection con, string path)
        {
            con.Open();
            var result = con.Execute(File.ReadAllText(path));
            con.Close();

            return result;
        }
    }   

    public class NpgsqlFixture : IDisposable
    {
        public const string ConnectionString = "User ID=postgres;Password=root;Host=localhost;Port=5432;Database=identitysrv_test;Pooling=false;";
        private const string ConnectionStringRoot = "User ID=postgres;Password=root;Host=localhost;Port=5432;Pooling=false;";

        public NpgsqlConnection Connection { get; set; }

        public NpgsqlFixture()
        {

        }

        public void Dispose()
        {
            if (this.Connection.State == System.Data.ConnectionState.Open)
            {
                this.Connection.Close();
            }

            this.Connection = null;
        }

        public void CreateDatabaseIfNotExists()
        {
            try
            {
                // Create connection to the server and create database 
                this.Connection = new NpgsqlConnection(ConnectionStringRoot);
                this.Connection.Execute(File.ReadAllText("./Services/Postgres/CreateDatabase.sql"));
            }
            catch (Exception)
            {
                // already exists 
            }

            // Connect the created database 
            this.Connection = new NpgsqlConnection(ConnectionString);
            this.Connection.ExecuteFromFile("./Services/Postgres/CreateTables.sql");
        }

        public void InsertData(string path)
        {
            this.Connection.ExecuteFromFile(path);
        }

        public void ClearDatabase()
        {
            this.Connection.ExecuteFromFile("./Services/Postgres/ClearTables.sql");
        }
    }
}
