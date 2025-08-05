using System.Data.Common;
using Biblioteca.Domain;
using Npgsql;
namespace Biblioteca.Storage;

public class CatalogStorage
{
    public void Create(Catalog catolog)
    {
        try
        {
            using var conn = DataBase.GetConnection();

            var cmd = new NpgsqlCommand("INSERT INTO catalog (id , created_at, updated_at, title, author, year, rev, publisher_id, pages, synopsis, language_id, is_foreign) VALUES (@id, @created_at, @updated_at, @title, @author, @year, @rev, @publisher_id, @pages, @synopsis, @language_id, @is_foreign)", conn);

            cmd.Parameters.AddWithValue("id", catolog.Id);
            cmd.Parameters.AddWithValue("created_at", catolog.CreatedAt);
            cmd.Parameters.AddWithValue("updated_at", catolog.UpdatedAt);
            cmd.Parameters.AddWithValue("title", catolog.Title);
            cmd.Parameters.AddWithValue("author", catolog.Author);
            cmd.Parameters.AddWithValue("year", catolog.Year);
            cmd.Parameters.AddWithValue("rev", catolog.Rev);
            cmd.Parameters.AddWithValue("publisher_id", catolog.PublisherId);
            cmd.Parameters.AddWithValue("pages", catolog.Pages);
            cmd.Parameters.AddWithValue("synopsis", (object?)catolog.Synopsis ?? DBNull.Value);
            cmd.Parameters.AddWithValue("language_id", catolog.LanguageId);
            cmd.Parameters.AddWithValue("is_foreign", catolog.IsForeign);

            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            var logPath = Path.Combine(logDir, "erros.txt");
            Console.WriteLine($"Gravando log em: {logPath}");
            File.AppendAllText(logPath, ex.ToString() + "\n");

        }
    }

    public List<Catalog> GetAll()
    {

    var catalogs = new List<Catalog>();

    using var conn = DataBase.GetConnection();
    var cmd = new NpgsqlCommand("SELECT id, created_at, updated_at, title, author, year, rev, publisher_id, pages, synopsis, language_id, is_foreign FROM catalog", conn);
    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        var catalog = new Catalog
        {
            Id = reader.GetString(0),
            CreatedAt = reader.GetDateTime(1),
            UpdatedAt = reader.GetDateTime(2),
            Title = reader.GetString(3),
            Author = reader.GetString(4),
            Year = reader.GetInt32(5),
            Rev = reader.GetInt32(6),
            PublisherId = reader.GetInt32(7),
            Pages = reader.GetInt32(8),
            Synopsis = reader.IsDBNull(9) ? null : reader.GetString(9),
            LanguageId = reader.GetInt32(10),
            IsForeign = reader.GetBoolean(11)
        };

        catalogs.Add(catalog);
    }

    return catalogs;
}

    


    public void Update(Catalog catalog)
    {
        using var conn = DataBase.GetConnection();
        var cmd = new NpgsqlCommand(@"
            UPDATE catalog SET 
                updated_at = NOW(),
                title = @title,
                author = @author,
                year = @year,
                rev = @rev,
                publisher_id = @publisher_id,
                pages = @pages,
                synopsis = @synopsis,
                language_id = @language_id,
                is_foreign = @is_foreign
            WHERE id = @id", conn);

        cmd.Parameters.AddWithValue("id", catalog.Id);
        cmd.Parameters.AddWithValue("title", catalog.Title);
        cmd.Parameters.AddWithValue("author", catalog.Author);
        cmd.Parameters.AddWithValue("year", catalog.Year);
        cmd.Parameters.AddWithValue("rev", catalog.Rev);
        cmd.Parameters.AddWithValue("publisher_id", catalog.PublisherId);
        cmd.Parameters.AddWithValue("pages", catalog.Pages);
        cmd.Parameters.AddWithValue("synopsis", catalog.Synopsis ?? "");
        cmd.Parameters.AddWithValue("language_id", catalog.LanguageId);
        cmd.Parameters.AddWithValue("is_foreign", catalog.IsForeign);

        cmd.ExecuteNonQuery();
    }

    public void Delete(string id)
    {
        using var conn = DataBase.GetConnection();
        var cmd = new NpgsqlCommand("DELETE FROM catalog WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }
}


