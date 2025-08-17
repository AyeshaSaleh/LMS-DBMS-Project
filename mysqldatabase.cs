// MySQLDatabase.cs
using MySql.Data.MySqlClient;
using System.Configuration;

public static class MySQLDatabase
{
    private static readonly string connectionString =
        ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

    public static MySqlConnection GetConnection()
        => new MySqlConnection(connectionString);
}