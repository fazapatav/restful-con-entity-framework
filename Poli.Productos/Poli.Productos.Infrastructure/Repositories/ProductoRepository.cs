using Microsoft.EntityFrameworkCore;
using Poli.Productos.Domain.Entities;
using Poli.Productos.Domain.Interfaces;
using Poli.Productos.Infrastructure.Data;

namespace Poli.Productos.Infrastructure.Repositories
{
    public class ProductoRepository : Repository<Producto>, IProductoRepository
    {
        public ProductoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Producto>> GetProductosByPrecioRangeAsync(decimal minPrecio, decimal maxPrecio)
        {
            return await _dbSet
                .Where(p => p.Precio >= minPrecio && p.Precio <= maxPrecio)
                .OrderBy(p => p.Precio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> SearchByNombreAsync(string nombre)
        {
            return await _dbSet
                .Where(p => p.Nombre.Contains(nombre))
                .ToListAsync();
        }
    }
}
