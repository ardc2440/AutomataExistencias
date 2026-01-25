using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IItemsMasterService
    {
        CatalogItem GetByItemId(int itemId);
        void UpdateVisibility(int itemId, bool visible);
    }
}
