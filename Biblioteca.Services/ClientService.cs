using Biblioteca.Domain;
using Biblioteca.Storage;

namespace Biblioteca.Services;

public class ClientService
{
    private readonly ClientStorage _storage = new ClientStorage();

    public void RegisterClient(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Name) || string.IsNullOrWhiteSpace(client.Email))
            throw new Exception("Nome e Email são obrigatórios.");
        _storage.Create(client);
    }

    public List<Client> ListClient()
    {
        var clients = _storage.GetAll();
        Console.WriteLine($"ListClient retornou {clients.Count} clientes.");
        return clients;

    }
    public void UpdateClient(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Name) || string.IsNullOrWhiteSpace(client.Email))
            throw new Exception("Nome e email são obrigatórios.");

        _storage.Update(client);
    }

    public void DeleteClient(string clientId)
    {
        _storage.Delete(clientId);
    }
}

