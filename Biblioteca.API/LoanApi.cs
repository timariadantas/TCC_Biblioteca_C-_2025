namespace Biblioteca.API;
using Biblioteca.Domain;
using Biblioteca.Services;

public class LoanApi
{
    private readonly LoanService _loanService;

    public LoanApi(LoanService loanService)
    {
        _loanService = loanService;
    }

    public void Menu()
    {
        while (true)
        {
            Console.WriteLine("\n--- EMPRÉSTIMOS ---");
            Console.WriteLine("1 - Cadastrar Empréstimo");
            Console.WriteLine("2 - Listar Empréstimos");
            Console.WriteLine("3 - Atualizar Empréstimo");
            Console.WriteLine("4 - Remover Empréstimo");
            Console.WriteLine("0 - Voltar");
            Console.Write("Escolha uma opção: ");
            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Cadastrar();
                    break;
                case "2":
                    Listar();
                    break;
                case "3":
                    Atualizar();
                    break;
                case "4":
                    Remover();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
    }

    private void Cadastrar()
    {
        try
        {
            var loan = new Loan();

            Console.Write("ID do Cliente: ");
            var clientIdInput = Console.ReadLine();
            if (clientIdInput is null)
            {
                Console.WriteLine("ID do Cliente não pode ser nulo.");
                return;
            }
            loan.ClientId = clientIdInput.Trim();

            Console.Write("ID do Inventário: ");
            var inventoryIdInput = Console.ReadLine();
            if (inventoryIdInput is null)
            {
                Console.WriteLine("ID do Inventário não pode ser nulo.");
                return;
            }
            loan.InventoryId = inventoryIdInput.Trim();

            Console.Write("Data do Empréstimo (yyyy-MM-dd): ");
            loan.LoanDate = DateTime.Parse(Console.ReadLine()!);

            Console.Write("Data de Devolução (yyyy-MM-dd): ");
            loan.DueDate = DateTime.Parse(Console.ReadLine()!);

            loan.ReturnDate = null; // inicialmente nulo
            loan.Status = "Aberto";
            loan.CreatedAt = DateTime.Now;
            loan.UpdatedAt = DateTime.Now;

            _loanService.CreateLoan(loan);
            Console.WriteLine("Empréstimo cadastrado com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    public void Listar()
    {
        try
        {
            var loans = _loanService.ListLoans();
            if (loans.Count == 0)
            {
                Console.WriteLine("Nenhum empréstimo encontrado.");
                return;
            }

            foreach (var loan in loans)
            {
                Console.WriteLine($"ID: {loan.Id} | Cliente: {loan.ClientId} | Inventário: {loan.InventoryId} | " +
                                  $"Empréstimo: {loan.LoanDate:yyyy-MM-dd} | Devolução: {loan.DueDate:yyyy-MM-dd} | " +
                                  $"Retorno: {(loan.ReturnDate.HasValue ? loan.ReturnDate.Value.ToString("yyyy-MM-dd") : "N/A")} | " +
                                  $"Status: {loan.Status}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    private void Atualizar()
    {
        try
        {
            Console.Write("ID do Empréstimo a atualizar: ");
            var id = Console.ReadLine();
            var loan = _loanService.GetLoanById(id!);

            Console.Write("Nova data de devolução (yyyy-MM-dd): ");
            loan.DueDate = DateTime.Parse(Console.ReadLine()!);

            Console.Write("Data de retorno (yyyy-MM-dd) ou vazio se não devolvido: ");
            var returnInput = Console.ReadLine();
            if (string.IsNullOrEmpty(returnInput))
                loan.ReturnDate = null;
            else
                loan.ReturnDate = DateTime.Parse(returnInput);


            Console.Write("Status (Aberto/Fechado): ");
            var statusInput = Console.ReadLine();
            if (statusInput is null)
            {
                Console.WriteLine("Status não pode ser nulo.");
                return;
            }
            loan.Status = statusInput;
            loan.UpdatedAt = DateTime.Now;

            _loanService.UpdateLoan(loan);
            Console.WriteLine("Empréstimo atualizado com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    private void Remover()
    {
        try
        {
            Console.Write("ID do Empréstimo a remover: ");
            var id = Console.ReadLine();
            _loanService.DeleteLoan(id!);
            Console.WriteLine("Empréstimo removido com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    public void Devolver()
    {
        Console.Write("ID do empréstimo: ");
        var id = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(id))
        {
            Console.WriteLine("ID Inválido.");
            return;
        }
        try
        {
            _loanService.ReturnLoan(id);
            Console.WriteLine("Devolução registrada com sucesso.");

        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao registra devolução: " + ex.Message);
        }

    }

    public void RelatorioEmprestimosPorCliente()
    {
        Console.Write("Informe o ID do Cliente: ");
        var clientId = Console.ReadLine();

        try
        {
            var loansWithStatus = _loanService.GetLoansByClientWithOverdue(clientId!);

            if (loansWithStatus.Count == 0)
            {
                Console.WriteLine("Nenhum empréstimo encontrado para este cliente.");
                return;
            }

            Console.WriteLine($"\nEmpréstimos do cliente {clientId}:");
            foreach (var (loan, isOverdue) in loansWithStatus)
            {
                string atraso = isOverdue ? " (ATRASADO!)" : "";
                Console.WriteLine($"ID: {loan.Id} | Item: {loan.InventoryId} | Devolução: {loan.DueDate:dd/MM/yyyy} | Status: {loan.Status}{atraso}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

public void RelatorioEmprestimosAtrasados()
{
    var overdueLoans = _loanService.GetOverdueLoans();

    if (!overdueLoans.Any())
    {
        Console.WriteLine("Não há empréstimos atrasados no momento.");
        return;
    }

    Console.WriteLine("\n--- Empréstimos Atrasados ---");
    foreach (var (loan, clientId) in overdueLoans)
    {
        Console.WriteLine($"ID: {loan.Id} | Cliente: {clientId} | Item: {loan.InventoryId} | " +
                          $"Data Empréstimo: {loan.LoanDate:dd/MM/yyyy} | Data Devolução: {loan.DueDate:dd/MM/yyyy} | Status: {loan.Status}");
    }
}


}

