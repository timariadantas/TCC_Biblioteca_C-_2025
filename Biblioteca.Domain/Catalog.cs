namespace Biblioteca.Domain;

public class Catalog
{
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Rev { get; set; }
    public int PublisherId { get; set; }
    public int Pages { get; set; }
    public string? Synopsis { get; set; }
    public int LanguageId { get; set; }
    public bool IsForeign { get; set; } = false;
}
