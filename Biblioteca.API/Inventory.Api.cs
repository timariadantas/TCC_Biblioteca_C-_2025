using Biblioteca.Domain;
using Biblioteca.Services;



namespace Biblioteca.API;

public class InventoryApi
{
    private readonly InventoryService _service;

    public InventoryApi(InventoryService service)
    {
        _service = service;
    }

    public void Menu()
    {
        while (true)
        {
            Console.WriteLine("\n--- INVENTÁRIO ---");
            Console.WriteLine("1 - Cadastrar Estoque");
            Console.WriteLine("2 - Listar Estoque");
            Console.WriteLine("3 - Atualizar Estoque");
            Console.WriteLine("4 - Remover Estoque");
            Console.WriteLine("0 - Voltar");
            Console.Write("Escolha uma opção: ");
            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1": Cadastrar(); break;
                case "2": Listar(); break;
                case "3": Atualizar(); break;
                case "4": Remover(); break;
                case "0": return;
                default: Console.WriteLine("Opção inválida!"); break;
            }
        }
    }

    private void Cadastrar()
    {
        Console.Write("ID do Livro (catalog_id): ");
        var catalogId = Console.ReadLine();

        Console.Write("Quantidade: ");
        if (!int.TryParse(Console.ReadLine(), out var quantity))
        {
            Console.WriteLine("Quantidade inválida!");
            return;
        }

        var inventory = new Inventory
        {
            CatalogId = catalogId,
            Quantity = quantity
        };

        _service.Create(inventory);
        Console.WriteLine("Estoque cadastrado com sucesso!");
    }

    private void Listar()
    {
        var itens = _service.GetAll();

        Console.WriteLine("\n--- ESTOQUE ---");
        foreach (var item in itens)
        {
            Console.WriteLine($"ID: {item.Id}");
            Console.WriteLine($"ID do Livro: {item.CatalogId}");
            Console.WriteLine($"Quantidade: {item.Quantity}");
            Console.WriteLine($"Criado em: {item.CreatedAt}");
            Console.WriteLine($"Atualizado em: {item.UpdatedAt}");
            Console.WriteLine("-------------------------");
        }
    }

    private void Atualizar()
{
    Console.Write("ID do Estoque a atualizar: ");
    var id = Console.ReadLine();

    var existing = _service.GetById(id);
    if (existing == null)
    {
        Console.WriteLine("Estoque não encontrado!");
        return;
    }

    Console.Write($"Nova Quantidade (atual {existing.Quantity}): ");
    if (!int.TryParse(Console.ReadLine(), out var quantity))
    {
        Console.WriteLine("Quantidade inválida!");
        return;
    }

    var inventory = new Inventory
    {
        Id = id,
        CatalogId = existing.CatalogId,
        CreatedAt = existing.CreatedAt,
        Shelf = existing.Shelf,
        Quantity = quantity
    };

    _service.Update(inventory);
    Console.WriteLine("Estoque atualizado com sucesso!");
}


    private void Remover()
    {
        Console.Write("ID do Estoque a remover: ");
        var id = Console.ReadLine();

        _service.Delete(id);
        Console.WriteLine("Estoque removido com sucesso!");
    }
}
