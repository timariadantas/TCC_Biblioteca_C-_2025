namespace Biblioteca.Domain;

public class Client
{
    public string Id { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";

}
