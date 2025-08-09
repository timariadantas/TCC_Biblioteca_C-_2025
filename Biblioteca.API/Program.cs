using Biblioteca.API;
using Biblioteca.Services;



var menu = new Menu();
var catalogApi = new CatalogApi();
var inventoryApi = new InventoryApi(new InventoryService());
var loanApi = new LoanApi(new LoanService());

while (true)
{
    Console.WriteLine("\n=== SISTEMA DA BIBLIOTECA MARIA ===");
    Console.WriteLine("1 - Gerenciar Clientes");
    Console.WriteLine("2 - Gerenciar Catálogo");
    Console.WriteLine("3 - Gerenciar Inventário");
    Console.WriteLine("4 - Gerenciar Emprestimo ");
    Console.WriteLine("0 - Sair");

    Console.Write("Escolha uma opção: ");
    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
            menu.Iniciar();
            break;
        case "2":
            catalogApi.Menu();
            break;
            case "3":
            inventoryApi.Menu();
            break;
            case "4":
            loanApi.Menu();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Opção inválida. Tente novamente.");
            break;
    }
}


    