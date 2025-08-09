using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Biblioteca.Storage;
using Biblioteca.Domain;
using Npgsql;

namespace Biblioteca.Storage;

public class InventoryStorage
{
    public void Create(Inventory inventory)
    {
        try
        {
            using var conn = DataBase.GetConnection();
            var cmd = new NpgsqlCommand(@"
            INSERT INTO inventory (id, created_at, updated_at, catalog_id, quantity, shelf) VALUES (@id, @created_at, @updated_at, @catalog_id, @quantity, @shelf)", conn);

            cmd.Parameters.AddWithValue("id", inventory.Id);
            cmd.Parameters.AddWithValue("created_at", inventory.CreatedAt);
            cmd.Parameters.AddWithValue("updated_at", inventory.UpdatedAt);
            cmd.Parameters.AddWithValue("catalog_id", inventory.CatalogId);
            cmd.Parameters.AddWithValue("quantity", inventory.Quantity);
            cmd.Parameters.AddWithValue("shelf", inventory.Shelf);

            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            var logPath = Path.Combine(logDir, "erros.txt");
            File.AppendAllText(logPath, ex.ToString() + "\n");

            Console.WriteLine("Erro: " + ex.Message); // <<< ADICIONADO


            throw new Exception("Erro ao inserir inventário ao banco de dados");
        }
    }

    public List<Inventory> GetAll()
    {
        var inventories = new List<Inventory>();

        using var conn = DataBase.GetConnection();

        var cmd = new NpgsqlCommand("SELECT * FROM inventory", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            inventories.Add(new Inventory
            {
                Id = reader.GetString(0),
                CatalogId = reader.GetString(1),
                CreatedAt = reader.GetDateTime(2),
                UpdatedAt = reader.GetDateTime(3),
                Quantity = reader.GetInt32(4),
                Shelf = reader.GetString(5)

            });
        }
        return inventories;

    }

    public void Update(Inventory inventory)
    {
        using var conn = DataBase.GetConnection();


        var cmd = new NpgsqlCommand(@"
        UPDATE inventory 
        SET updated_at = @updated_at,
            catalog_id = @catalog_id,
            quantity = @quantity,
            shelf = @shelf
            WHERE id = @id", conn);

        cmd.Parameters.AddWithValue("id", inventory.Id);
        cmd.Parameters.AddWithValue("updated_at", inventory.UpdatedAt);
        cmd.Parameters.AddWithValue("catalog_id", inventory.CatalogId);
        cmd.Parameters.AddWithValue("quantity", inventory.Quantity);
        cmd.Parameters.AddWithValue("shelf", inventory.Shelf);

        cmd.ExecuteNonQuery();
    }

    public void Delete(string id)
    {
        using var conn = DataBase.GetConnection();


        var cmd = new NpgsqlCommand("DELETE FROM inventory WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        cmd.ExecuteNonQuery();
    }

    public Inventory? GetById(string id)
    {
        try
        {
            using var conn = DataBase.GetConnection();

            var cmd = new NpgsqlCommand(
                "SELECT id, catalog_id, created_at, updated_at, quantity, shelf " +
                "FROM inventory WHERE id = @id",
                conn
            );
            cmd.Parameters.AddWithValue("id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Inventory
                {
                    Id = reader.GetString(0),
                    CatalogId = reader.GetString(1),
                    CreatedAt = reader.GetDateTime(2),
                    UpdatedAt = reader.GetDateTime(3),
                    Quantity = reader.GetInt32(4),
                    Shelf = reader.GetString(5)
                };
            }

            return null;

        }
        catch (Exception ex)
        {

            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            var logPath = Path.Combine(logDir, "erros.txt");
            File.AppendAllText(logPath, ex.ToString() + "\n");

            Console.WriteLine("Erro: " + ex.Message); // <<< ADICIONADO


            throw new Exception("Erro !");
        }
    }
}
