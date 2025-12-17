using Poli.Productos.Domain.Entities;

namespace Poli.Productos.Domain.Interfaces
{
    public interface IProductoRepository : IRepository<Producto>
    {
        Task<IEnumerable<Producto>> GetProductosByPrecioRangeAsync(decimal minPrecio, decimal maxPrecio);
        Task<IEnumerable<Producto>> SearchByNombreAsync(string nombre);
    }
}
