using AutoMapper;
using Poli.Productos.Aplication.Dtos;
using Poli.Productos.Aplication.Interfaces;
using Poli.Productos.Domain.Entities;
using Poli.Productos.Domain.Interfaces;

namespace Poli.Productos.Aplication.UsesCase
{
    public class ProductoService : IProductoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductoDto>> GetAllProductosAsync()
        {
            var productos = await _unitOfWork.Productos.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }

        public async Task<ProductoDto?> GetProductoByIdAsync(int id)
        {
            var producto = await _unitOfWork.Productos.GetByIdAsync(id);
            return producto == null ? null : _mapper.Map<ProductoDto>(producto);
        }

        public async Task<ProductoDto> CreateProductoAsync(ProductoCreateDto productoDto)
        {
            var producto = _mapper.Map<Producto>(productoDto);
            var createdProducto = await _unitOfWork.Productos.AddAsync(producto);
            return _mapper.Map<ProductoDto>(createdProducto);
        }

        public async Task<ProductoDto> UpdateProductoAsync(int id, ProductoUpdateDto productoDto)
        {
            var existingProducto = await _unitOfWork.Productos.GetByIdAsync(id);

            if (existingProducto == null)
            {
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
            }

            _mapper.Map(productoDto, existingProducto);
            await _unitOfWork.Productos.UpdateAsync(existingProducto);

            return _mapper.Map<ProductoDto>(existingProducto);
        }

        public async Task<bool> DeleteProductoAsync(int id)
        {
            var exists = await _unitOfWork.Productos.ExistsAsync(id);

            if (!exists)
            {
                return false;
            }

            await _unitOfWork.Productos.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ProductoDto>> SearchProductosByNombreAsync(string nombre)
        {
            var productos = await _unitOfWork.Productos.SearchByNombreAsync(nombre);
            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }

        public async Task<IEnumerable<ProductoDto>> GetProductosByPrecioRangeAsync(decimal minPrecio, decimal maxPrecio)
        {
            var productos = await _unitOfWork.Productos.GetProductosByPrecioRangeAsync(minPrecio, maxPrecio);
            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }
    }
}
