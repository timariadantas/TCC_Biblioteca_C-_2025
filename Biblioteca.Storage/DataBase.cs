using Npgsql;
namespace Biblioteca.Storage;

public static class DataBase
{
    private const string CONNECTION_STRING = "Host=localhost;Username=seu_usuario;Password=sua_senha;Database=seubanco";

    public static NpgsqlConnection GetConnection()
    {
        var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        return conn;

    }
}