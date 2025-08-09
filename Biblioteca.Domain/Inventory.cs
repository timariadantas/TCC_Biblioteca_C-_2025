namespace Biblioteca.Domain;

public class Inventory
{
    public string Id { get; set; } = string.Empty;
    public string CatalogId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public int Quantity { get; set; }
    public string Shelf { get; set; } = string.Empty;

}


