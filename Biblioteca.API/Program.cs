using Biblioteca.API;
using Biblioteca.Services;
using Biblioteca.Storage;



var menu = new Menu();
var catalogApi = new CatalogApi();
var inventoryApi = new InventoryApi(new InventoryService());
var loanApi = new LoanApi(new LoanService());

while (true)
{
    Console.WriteLine("\n=== SEJA BEM VINDO À BIBLIOTECA DA MARIA ===");
    Console.WriteLine("1 - Gerenciar Clientes");
    Console.WriteLine("2 - Gerenciar Catálogo");
    Console.WriteLine("3 - Gerenciar Inventário");
    Console.WriteLine("4 - Gerenciar Emprestimo ");
    Console.WriteLine("5 - Devolver");
    Console.WriteLine("6 - Listar Cliente");
    Console.WriteLine("7 - Relatório Por Cliente");
    Console.WriteLine("8 - Relatório Por Cliente atrasados");

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
            case "5":
            loanApi.Devolver();
            break;
            case "6":
            loanApi.Listar();
            break;
            case "7":
            loanApi.RelatorioEmprestimosPorCliente();
            break;
            case"8":
            loanApi.RelatorioEmprestimosAtrasados();
            break;

            case "0":
            return;
        default:
            Console.WriteLine("Opção inválida. Tente novamente.");
            break;
    }
}


    