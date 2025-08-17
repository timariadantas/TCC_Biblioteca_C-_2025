using Npgsql;
namespace Biblioteca.Storage;
// Usando boas práticas de Singleton ,, garante que uma única instância da classe durante a execuçao do programa.
// Singleton não deve ser alterado por herança.
public sealed class DataBase
{
    private static DataBase? _instance;
    private static readonly object _lock = new object();
    private readonly NpgsqlConnection _connection;

    private DataBase()
    {
        const string CONNECTION_STRING =
        "host=localhost;Username=--;Password=--;Database=--";

        _connection = new NpgsqlConnection(CONNECTION_STRING);
        _connection.Open();
    }

    public static DataBase Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = new DataBase();

                return _instance;
            }
        }
    }

    public NpgsqlConnection GetConnection()
    {
        if (_connection.State != System.Data.ConnectionState.Open)
            _connection.Open();

        return _connection;
    }
}

// _instance → guarda a única instância do DataBase.
//_lock → usado para evitar que duas threads criem duas instâncias ao mesmo tempo (thread safety).
//_connection → a conexão aberta com o PostgreSQL.
// Esse duplo if (_instance == null) é chamado Double-Checked Locking — evita criar duas instâncias por acidente.