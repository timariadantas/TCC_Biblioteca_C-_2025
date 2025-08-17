using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Biblioteca.Domain;
using Npgsql;
namespace Biblioteca.Storage;

public class ClientStorage
{
    public void Create(Client client)
    {
        try
        {
            using var conn = DataBase.Instance.GetConnection();
            Console.WriteLine("Conexão aberta para inserir dados");
            var cmd = new NpgsqlCommand("INSERT INTO client (id, created_at, updated_at, name, email, phone) VALUES (@id, @created, @updated, @name, @email, @phone)", conn);

            cmd.Parameters.AddWithValue("@id", client.Id);
            cmd.Parameters.AddWithValue("@created", client.CreatedAt);
            cmd.Parameters.AddWithValue("@updated", client.UpdatedAt);
            cmd.Parameters.AddWithValue("@name", client.Name);
            cmd.Parameters.AddWithValue("@email", client.Email);
            cmd.Parameters.AddWithValue("@phone", client.Phone);

            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            var logPath = Path.Combine(logDir, "erros.txt");
            File.AppendAllText(logPath, ex.ToString() + "\n");
        }

    }

    public List<Client> GetAll()
    {
        var clients = new List<Client>();

        try
        {
            using var conn = DataBase.Instance.GetConnection();
            var cmd = new NpgsqlCommand("SELECT id, created_at, updated_at, name, email, phone FROM public.client", conn);

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var client = new Client
                {
                    Id = reader.GetString(0),
                    CreatedAt = reader.GetDateTime(1),
                    UpdatedAt = reader.GetDateTime(2),
                    Name = reader.GetString(3),
                    Email = reader.GetString(4),
                    Phone = reader.GetString(5)
                };

                clients.Add(client);
            }
            Console.WriteLine($"Foram encontrados {clients.Count} clientes no banco.");
        }
        catch (Exception ex)
        {
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            var logPath = Path.Combine(logDir, "erros.txt");
            File.AppendAllText(logPath, ex.ToString() + "\n");
            throw;
        }
        return clients;
    }

    public void Update(Client client)
    {
        try
        {
            using var conn = DataBase.Instance.GetConnection();
            var cmd = new NpgsqlCommand(
                @"UPDATE client
                SET updated_at = @updated, name = @name, email = @email, phone = @phone
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@id", client.Id);
            cmd.Parameters.AddWithValue("@updated", DateTime.Now);
            cmd.Parameters.AddWithValue("@name", client.Name);
            cmd.Parameters.AddWithValue("@email", client.Email);
            cmd.Parameters.AddWithValue("@phone", client.Phone);

            var rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected == 0)
                throw new Exception("Cliente não encontrado para atualizar.");

        }
        catch (Exception ex)
        {
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            var logPath = Path.Combine(logDir, "erros.txt");
            File.AppendAllText(logPath, ex.ToString() + "\n");
            throw;
        }
    }

    public void Delete(string clientId)
    {
        try
        {
            using var conn = DataBase.Instance.GetConnection();

            var cmd = new NpgsqlCommand("DELETE FROM client WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", clientId);

            var rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected == 0)
                throw new Exception("Cliente não encontrado para deletar.");

        }
        catch (Exception ex)
        {
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            var logPath = Path.Combine(logDir, "erros.txt");
            File.AppendAllText(logPath, ex.ToString() + "\n");
            throw;
        }
    }

}

