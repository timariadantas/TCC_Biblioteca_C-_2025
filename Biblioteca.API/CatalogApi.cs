using Biblioteca.Domain;
using Biblioteca.Services;

namespace Biblioteca.API;

public class CatalogApi
{
    private readonly CatalogService _service;

    public CatalogApi()
    {
        _service = new CatalogService();
    }

    public void Menu()
    {
        while (true)
        {
            Console.WriteLine("\n--- CATÁLOGO ---");
            Console.WriteLine("1 - Cadastrar Livro");
            Console.WriteLine("2 - Listar Catálogo");
            Console.WriteLine("3 - Atualizar Livro");
            Console.WriteLine("4 - Remover Livro");
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
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }

    private void Cadastrar()
    {
        Console.WriteLine("\n--- Novo Livro ---");

        Console.Write("ISBN: ");
        var id = Console.ReadLine();

        Console.Write("Título: ");
        var title = Console.ReadLine();

        Console.Write("Autor: ");
        var author = Console.ReadLine();

        Console.Write("Ano: ");
        var year = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Edição: ");
        var rev = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Editora ID: ");
        var publisherId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Páginas: ");
        var pages = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Sinopse: ");
        var synopsis = Console.ReadLine();

        Console.Write("Idioma ID: ");
        var languageId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("É estrangeiro? (s/n): ");
        var foreign = Console.ReadLine()?.ToLower() == "s";

        var catalog = new Catalog
        {
            Id = id!,
            Title = title!,
            Author = author!,
            Year = year,
            Rev = rev,
            PublisherId = publisherId,
            Pages = pages,
            Synopsis = synopsis,
            LanguageId = languageId,
            IsForeign = foreign,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        try
        {
            _service.CreateCatalog(catalog);
            Console.WriteLine("Livro cadastrado com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao cadastrar livro: " + ex.Message);
        }
    }

    private void Listar()
    {
        var lista = _service.GetAllCatalogs();

        Console.WriteLine("\n--- CATÁLOGO DE LIVROS ---");
        foreach (var c in lista)
        {
            Console.WriteLine($"ISBN: {c.Id} | Título: {c.Title} | Autor: {c.Author} | Ano: {c.Year} | EditoraID: {c.PublisherId}");
        }

        if (lista.Count == 0)
        {
            Console.WriteLine("Nenhum livro encontrado.");
        }
    }

    private void Atualizar()
    {
        Console.WriteLine("\n--- Atualizar Livro ---");

        Console.Write("ISBN do livro: ");
        var id = Console.ReadLine();

        Console.Write("Novo Título: ");
        var title = Console.ReadLine();

        Console.Write("Novo Autor: ");
        var author = Console.ReadLine();

        Console.Write("Novo Ano: ");
        var year = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Nova Edição: ");
        var rev = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Nova Editora ID: ");
        var publisherId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Novas Páginas: ");
        var pages = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Nova Sinopse: ");
        var synopsis = Console.ReadLine();

        Console.Write("Novo Idioma ID: ");
        var languageId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("É estrangeiro? (s/n): ");
        var foreign = Console.ReadLine()?.ToLower() == "s";

        var catalog = new Catalog
        {
            Id = id!,
            Title = title!,
            Author = author!,
            Year = year,
            Rev = rev,
            PublisherId = publisherId,
            Pages = pages,
            Synopsis = synopsis,
            LanguageId = languageId,
            IsForeign = foreign,
            UpdatedAt = DateTime.Now
        };

        try
        {
            _service.UpdateCatalog(catalog);
            Console.WriteLine("Livro atualizado com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao atualizar livro: " + ex.Message);
        }
    }

    private void Remover()
    {
        Console.Write("\nDigite o ISBN do livro para remover: ");
        var id = Console.ReadLine();

        try
        {
            _service.DeleteCatalog(id!);
            Console.WriteLine("Livro removido com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao remover livro: " + ex.Message);
        }
    }
}
