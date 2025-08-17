using Npgsql;

namespace Biblioteca.Storage;

public sealed class DataBase
{
    private static DataBase? _instance;
    private static readonly object _lock = new object();

    private DataBase() { }

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
        const string CONNECTION_STRING =
            "host=localhost;Username=usuario;Password=senha;Database=banco";

        var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        return conn; // retorna uma conexão nova a cada chamada
    }
}

// O Singleton deve ser apenas para a classe DataBase, e cada chamada de GetConnection() deve criar uma conexão nova e aberta.
// _instance → guarda a única instância do DataBase.
//_lock → usado para evitar que duas threads criem duas instâncias ao mesmo tempo (thread safety).
// Esse duplo if (_instance == null) é chamado Double-Checked Locking — evita criar duas instâncias por acidente.