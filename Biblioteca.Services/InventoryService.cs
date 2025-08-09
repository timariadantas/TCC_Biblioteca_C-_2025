using Biblioteca.Domain;
using Biblioteca.Storage;
using NUlid;

namespace Biblioteca.Services;

public class InventoryService
{
    private readonly InventoryStorage _storage;

    public InventoryService()
    {
        _storage = new InventoryStorage();
    }

    public void Create(Inventory inventory)
    {
        inventory.Id = Ulid.NewUlid().ToString("N", null)[..26];
        inventory.CreatedAt = DateTime.Now;
        inventory.UpdatedAt = DateTime.Now;

        if (string.IsNullOrEmpty(inventory.CatalogId))
            throw new Exception("É necessário informar o ID do catálogo.");

        _storage.Create(inventory);
    }

    public List<Inventory> GetAll()
    {
        return _storage.GetAll();
    }

    public Inventory? GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID inválido.");

        return _storage.GetById(id);
    }

    public void Update(Inventory inventory)
{
    var existing = _storage.GetById(inventory.Id);
    if (existing == null)
        throw new Exception("Inventário não encontrado.");

    // Aqui você deve garantir que CatalogId, CreatedAt e Shelf sejam preservados
    inventory.CatalogId = existing.CatalogId;
    inventory.CreatedAt = existing.CreatedAt;
    inventory.Shelf = existing.Shelf;

    inventory.UpdatedAt = DateTime.Now;

    _storage.Update(inventory);
}

    public void Delete(string id)
    {
        var existing = _storage.GetById(id);
        if (existing == null)
            throw new Exception("Inventário não encontrado.");

        _storage.Delete(id);
    }
}