using Npgsql;
namespace Biblioteca.Storage;

public static class DataBase
{
    private const string CONNECTION_STRING = "Host=;Username=--;Password=--;Database=--";

    public static NpgsqlConnection GetConnection()
    {
        var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        return conn;

    }
}