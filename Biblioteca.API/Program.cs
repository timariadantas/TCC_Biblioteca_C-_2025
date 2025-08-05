using Biblioteca.API;
using Biblioteca.Services;



var menu = new Menu();
var catalogApi = new CatalogApi();

while (true)
{
    Console.WriteLine("\n=== SISTEMA DA BIBLIOTECA ===");
    Console.WriteLine("1 - Gerenciar Clientes");
    Console.WriteLine("2 - Gerenciar Catálogo");
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
        case "0":
            Console.WriteLine("Encerrando o sistema...");
            return;
        default:
            Console.WriteLine("Opção inválida. Tente novamente.");
            break;
    }
}


    