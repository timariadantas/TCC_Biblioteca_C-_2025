namespace Biblioteca.Domain;

public class Loan
{

    public string Id { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string InventoryId { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; } = null;
    public string Status { get; set; } = "Aberto"; // fazer um enum depois!!!
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}

