using AutoMapper;
using Moq;
using Poli.Productos.Aplication.Dtos;
using Poli.Productos.Aplication.UsesCase;
using Poli.Productos.Domain.Entities;
using Poli.Productos.Domain.Interfaces;
using Xunit;

namespace Poli.Productos.Test.Services
{
    public class ProductoServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProductoRepository> _mockProductoRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductoService _service;

        public ProductoServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductoRepository = new Mock<IProductoRepository>();
            _mockMapper = new Mock<IMapper>();

            _mockUnitOfWork.Setup(u => u.Productos).Returns(_mockProductoRepository.Object);
            _service = new ProductoService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        #region GetAllProductosAsync

        [Fact]
        public async Task GetAllProductosAsync_DebeRetornarListaDeProductos()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Producto 1", Descripcion = "Desc 1", Precio = 100m },
                new Producto { Id = 2, Nombre = "Producto 2", Descripcion = "Desc 2", Precio = 200m }
            };

            var productosDto = new List<ProductoDto>
            {
                new ProductoDto { Id = 1, Nombre = "Producto 1", Descripcion = "Desc 1", Precio = 100m },
                new ProductoDto { Id = 2, Nombre = "Producto 2", Descripcion = "Desc 2", Precio = 200m }
            };

            _mockProductoRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(productos);
            _mockMapper.Setup(m => m.Map<IEnumerable<ProductoDto>>(productos)).Returns(productosDto);

            // Act
            var resultado = await _service.GetAllProductosAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            _mockProductoRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<ProductoDto>>(productos), Times.Once);
        }

        [Fact]
        public async Task GetAllProductosAsync_DebeRetornarListaVacia_CuandoNoHayProductos()
        {
            // Arrange
            var productosVacios = new List<Producto>();
            var productosDtoVacios = new List<ProductoDto>();

            _mockProductoRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(productosVacios);
            _mockMapper.Setup(m => m.Map<IEnumerable<ProductoDto>>(productosVacios)).Returns(productosDtoVacios);

            // Act
            var resultado = await _service.GetAllProductosAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        #endregion

        #region GetProductoByIdAsync

        [Fact]
        public async Task GetProductoByIdAsync_DebeRetornarProducto_CuandoExiste()
        {
            // Arrange
            var producto = new Producto { Id = 1, Nombre = "Producto 1", Descripcion = "Desc 1", Precio = 100m };
            var productoDto = new ProductoDto { Id = 1, Nombre = "Producto 1", Descripcion = "Desc 1", Precio = 100m };

            _mockProductoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(producto);
            _mockMapper.Setup(m => m.Map<ProductoDto>(producto)).Returns(productoDto);

            // Act
            var resultado = await _service.GetProductoByIdAsync(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Producto 1", resultado.Nombre);
        }

        [Fact]
        public async Task GetProductoByIdAsync_DebeRetornarNull_CuandoNoExiste()
        {
            // Arrange
            _mockProductoRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Producto?)null);

            // Act
            var resultado = await _service.GetProductoByIdAsync(999);

            // Assert
            Assert.Null(resultado);
            _mockMapper.Verify(m => m.Map<ProductoDto>(It.IsAny<Producto>()), Times.Never);
        }

        #endregion

        #region CreateProductoAsync

        [Fact]
        public async Task CreateProductoAsync_DebeCrearYRetornarProducto()
        {
            // Arrange
            var createDto = new ProductoCreateDto
            {
                Nombre = "Nuevo Producto",
                Descripcion = "Nueva Descripción",
                Precio = 150m
            };

            var producto = new Producto
            {
                Nombre = "Nuevo Producto",
                Descripcion = "Nueva Descripción",
                Precio = 150m
            };

            var productoCreado = new Producto
            {
                Id = 1,
                Nombre = "Nuevo Producto",
                Descripcion = "Nueva Descripción",
                Precio = 150m
            };

            var productoDto = new ProductoDto
            {
                Id = 1,
                Nombre = "Nuevo Producto",
                Descripcion = "Nueva Descripción",
                Precio = 150m
            };

            _mockMapper.Setup(m => m.Map<Producto>(createDto)).Returns(producto);
            _mockProductoRepository.Setup(r => r.AddAsync(producto)).ReturnsAsync(productoCreado);
            _mockMapper.Setup(m => m.Map<ProductoDto>(productoCreado)).Returns(productoDto);

            // Act
            var resultado = await _service.CreateProductoAsync(createDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Nuevo Producto", resultado.Nombre);
            _mockProductoRepository.Verify(r => r.AddAsync(It.IsAny<Producto>()), Times.Once);
        }

        #endregion

        #region UpdateProductoAsync

        [Fact]
        public async Task UpdateProductoAsync_DebeActualizarYRetornarProducto_CuandoExiste()
        {
            // Arrange
            var updateDto = new ProductoUpdateDto
            {
                Nombre = "Producto Actualizado",
                Descripcion = "Descripción Actualizada",
                Precio = 200m
            };

            var productoExistente = new Producto
            {
                Id = 1,
                Nombre = "Producto Original",
                Descripcion = "Descripción Original",
                Precio = 100m
            };

            var productoDto = new ProductoDto
            {
                Id = 1,
                Nombre = "Producto Actualizado",
                Descripcion = "Descripción Actualizada",
                Precio = 200m
            };

            _mockProductoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(productoExistente);
            _mockMapper.Setup(m => m.Map(updateDto, productoExistente)).Returns(productoExistente);
            _mockProductoRepository.Setup(r => r.UpdateAsync(productoExistente)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<ProductoDto>(productoExistente)).Returns(productoDto);

            // Act
            var resultado = await _service.UpdateProductoAsync(1, updateDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Producto Actualizado", resultado.Nombre);
            _mockProductoRepository.Verify(r => r.UpdateAsync(It.IsAny<Producto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductoAsync_DebeLanzarKeyNotFoundException_CuandoNoExiste()
        {
            // Arrange
            var updateDto = new ProductoUpdateDto
            {
                Nombre = "Producto",
                Descripcion = "Descripción",
                Precio = 100m
            };

            _mockProductoRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Producto?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.UpdateProductoAsync(999, updateDto)
            );

            Assert.Contains("999", exception.Message);
            Assert.Contains("no encontrado", exception.Message);
            _mockProductoRepository.Verify(r => r.UpdateAsync(It.IsAny<Producto>()), Times.Never);
        }

        #endregion

        #region DeleteProductoAsync

        [Fact]
        public async Task DeleteProductoAsync_DebeRetornarTrue_CuandoProductoExiste()
        {
            // Arrange
            _mockProductoRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _mockProductoRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.DeleteProductoAsync(1);

            // Assert
            Assert.True(resultado);
            _mockProductoRepository.Verify(r => r.ExistsAsync(1), Times.Once);
            _mockProductoRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteProductoAsync_DebeRetornarFalse_CuandoProductoNoExiste()
        {
            // Arrange
            _mockProductoRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

            // Act
            var resultado = await _service.DeleteProductoAsync(999);

            // Assert
            Assert.False(resultado);
            _mockProductoRepository.Verify(r => r.ExistsAsync(999), Times.Once);
            _mockProductoRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        #endregion

        #region SearchProductosByNombreAsync

        [Fact]
        public async Task SearchProductosByNombreAsync_DebeRetornarProductosCoincidentes()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Laptop HP", Descripcion = "Desc", Precio = 1000m },
                new Producto { Id = 2, Nombre = "Laptop Dell", Descripcion = "Desc", Precio = 900m }
            };

            var productosDto = new List<ProductoDto>
            {
                new ProductoDto { Id = 1, Nombre = "Laptop HP", Descripcion = "Desc", Precio = 1000m },
                new ProductoDto { Id = 2, Nombre = "Laptop Dell", Descripcion = "Desc", Precio = 900m }
            };

            _mockProductoRepository.Setup(r => r.SearchByNombreAsync("Laptop")).ReturnsAsync(productos);
            _mockMapper.Setup(m => m.Map<IEnumerable<ProductoDto>>(productos)).Returns(productosDto);

            // Act
            var resultado = await _service.SearchProductosByNombreAsync("Laptop");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            _mockProductoRepository.Verify(r => r.SearchByNombreAsync("Laptop"), Times.Once);
        }

        [Fact]
        public async Task SearchProductosByNombreAsync_DebeRetornarListaVacia_SinCoincidencias()
        {
            // Arrange
            var productosVacios = new List<Producto>();
            var productosDtoVacios = new List<ProductoDto>();

            _mockProductoRepository.Setup(r => r.SearchByNombreAsync("NoExiste")).ReturnsAsync(productosVacios);
            _mockMapper.Setup(m => m.Map<IEnumerable<ProductoDto>>(productosVacios)).Returns(productosDtoVacios);

            // Act
            var resultado = await _service.SearchProductosByNombreAsync("NoExiste");

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        #endregion

        #region GetProductosByPrecioRangeAsync

        [Fact]
        public async Task GetProductosByPrecioRangeAsync_DebeRetornarProductosEnRango()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Producto 1", Descripcion = "Desc", Precio = 150m },
                new Producto { Id = 2, Nombre = "Producto 2", Descripcion = "Desc", Precio = 300m }
            };

            var productosDto = new List<ProductoDto>
            {
                new ProductoDto { Id = 1, Nombre = "Producto 1", Descripcion = "Desc", Precio = 150m },
                new ProductoDto { Id = 2, Nombre = "Producto 2", Descripcion = "Desc", Precio = 300m }
            };

            _mockProductoRepository.Setup(r => r.GetProductosByPrecioRangeAsync(100m, 500m)).ReturnsAsync(productos);
            _mockMapper.Setup(m => m.Map<IEnumerable<ProductoDto>>(productos)).Returns(productosDto);

            // Act
            var resultado = await _service.GetProductosByPrecioRangeAsync(100m, 500m);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.All(resultado, p => Assert.InRange(p.Precio, 100m, 500m));
            _mockProductoRepository.Verify(r => r.GetProductosByPrecioRangeAsync(100m, 500m), Times.Once);
        }

        [Fact]
        public async Task GetProductosByPrecioRangeAsync_DebeRetornarListaVacia_SinProductosEnRango()
        {
            // Arrange
            var productosVacios = new List<Producto>();
            var productosDtoVacios = new List<ProductoDto>();

            _mockProductoRepository.Setup(r => r.GetProductosByPrecioRangeAsync(5000m, 10000m)).ReturnsAsync(productosVacios);
            _mockMapper.Setup(m => m.Map<IEnumerable<ProductoDto>>(productosVacios)).Returns(productosDtoVacios);

            // Act
            var resultado = await _service.GetProductosByPrecioRangeAsync(5000m, 10000m);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        #endregion
    }
}
