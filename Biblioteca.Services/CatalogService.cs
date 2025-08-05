using System.Security.Cryptography.X509Certificates;
using Biblioteca.Domain;
using Biblioteca.Storage;

namespace Biblioteca.Services;

public class CatalogService
{
    private readonly CatalogStorage _storage;

    public CatalogService()
    {
        _storage = new CatalogStorage();
    }

    public void CreateCatalog(Catalog catalog)
    {
        if (string.IsNullOrWhiteSpace(catalog.Id) || string.IsNullOrWhiteSpace(catalog.Title))
        {
            throw new ArgumentException("Id e Titulo são obrigatórios.");

        }
        _storage.Create(catalog);
    }

    public List<Catalog> GetAllCatalogs()
    {
        return _storage.GetAll();
    }

    public void UpdateCatalog(Catalog catalog)
    {
        _storage.Update(catalog);
    }

    public void DeleteCatalog(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Id inválido");

        _storage.Delete(id);
    }
}
