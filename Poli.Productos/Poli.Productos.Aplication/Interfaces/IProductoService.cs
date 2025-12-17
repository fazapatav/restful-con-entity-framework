using Poli.Productos.Aplication.Dtos;

namespace Poli.Productos.Aplication.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDto>> GetAllProductosAsync();
        Task<ProductoDto?> GetProductoByIdAsync(int id);
        Task<ProductoDto> CreateProductoAsync(ProductoCreateDto productoDto);
        Task<ProductoDto> UpdateProductoAsync(int id, ProductoUpdateDto productoDto);
        Task<bool> DeleteProductoAsync(int id);
        Task<IEnumerable<ProductoDto>> SearchProductosByNombreAsync(string nombre);
        Task<IEnumerable<ProductoDto>> GetProductosByPrecioRangeAsync(decimal minPrecio, decimal maxPrecio);
    }
}
