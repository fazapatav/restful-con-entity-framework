using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Poli.Productos.Api.Controllers;
using Poli.Productos.Aplication.Common;
using Poli.Productos.Aplication.Dtos;
using Poli.Productos.Aplication.Interfaces;
using Xunit;

namespace Poli.Productos.Test.Controllers
{
    public class ProductoControllerTests
    {
        private readonly Mock<IProductoService> _mockProductoService;
        private readonly Mock<ILogger<ProductoController>> _mockLogger;
        private readonly ProductoController _controller;

        public ProductoControllerTests()
        {
            _mockProductoService = new Mock<IProductoService>();
            _mockLogger = new Mock<ILogger<ProductoController>>();
            _controller = new ProductoController(_mockProductoService.Object, _mockLogger.Object);
        }

        #region GetAllProductos - GET /api/producto

        [Fact]
        public async Task GetAllProductos_DebeRetornar200ConProductos()
        {
            // Arrange
            var productosEsperados = new List<ProductoDto>
            {
                new ProductoDto { Id = 1, Nombre = "Laptop", Descripcion = "Laptop HP", Precio = 15000m },
                new ProductoDto { Id = 2, Nombre = "Mouse", Descripcion = "Mouse Logitech", Precio = 350m }
            };

            _mockProductoService.Setup(s => s.GetAllProductosAsync())
                .ReturnsAsync(productosEsperados);

            // Act
            var resultado = await _controller.GetAllProductos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProductoDto>>>(okResult.Value);

            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.Data.Count());
        }

        [Fact]
        public async Task GetAllProductos_DebeRetornarListaVacia_CuandoNoHayProductos()
        {
            // Arrange
            _mockProductoService.Setup(s => s.GetAllProductosAsync())
                .ReturnsAsync(new List<ProductoDto>());

            // Act
            var resultado = await _controller.GetAllProductos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProductoDto>>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Empty(response.Data);
        }

        #endregion

        #region GetProductoById - GET /api/producto/{id}

        [Fact]
        public async Task GetProductoById_DebeRetornar200_CuandoProductoExiste()
        {
            // Arrange
            var producto = new ProductoDto
            {
                Id = 1,
                Nombre = "Monitor",
                Descripcion = "Monitor 24 pulgadas",
                Precio = 3500m
            };

            _mockProductoService.Setup(s => s.GetProductoByIdAsync(1))
                .ReturnsAsync(producto);

            // Act
            var resultado = await _controller.GetProductoById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<ProductoDto>>(okResult.Value);

            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);
        }

        [Fact]
        public async Task GetProductoById_DebeRetornar404_CuandoProductoNoExiste()
        {
            // Arrange
            _mockProductoService.Setup(s => s.GetProductoByIdAsync(999))
                .ReturnsAsync((ProductoDto?)null);

            // Act
            var resultado = await _controller.GetProductoById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<ProductoDto>>(notFoundResult.Value);
            Assert.False(response.Success);
        }

        #endregion

        #region CreateProducto - POST /api/producto

        [Fact]
        public async Task CreateProducto_DebeRetornar201_CuandoDatosValidos()
        {
            // Arrange
            var nuevoProducto = new ProductoCreateDto
            {
                Nombre = "Impresora",
                Descripcion = "Impresora láser HP",
                Precio = 4500m
            };

            var productoCreado = new ProductoDto
            {
                Id = 10,
                Nombre = "Impresora",
                Descripcion = "Impresora láser HP",
                Precio = 4500m
            };

            _mockProductoService.Setup(s => s.CreateProductoAsync(nuevoProducto))
                .ReturnsAsync(productoCreado);

            // Act
            var resultado = await _controller.CreateProducto(nuevoProducto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<ProductoDto>>(createdResult.Value);

            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(10, response.Data.Id);
            Assert.Equal(nameof(ProductoController.GetProductoById), createdResult.ActionName);
        }

        [Fact]
        public async Task CreateProducto_DebeRetornar400_CuandoValidacionFalla()
        {
            // Arrange
            var productoInvalido = new ProductoCreateDto
            {
                Nombre = "Test",
                Descripcion = "Test",
                Precio = 100m
            };

            _controller.ModelState.AddModelError("Nombre", "El nombre es requerido");

            // Act
            var resultado = await _controller.CreateProducto(productoInvalido);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<ProductoDto>>(badRequestResult.Value);

            Assert.False(response.Success);
            Assert.NotEmpty(response.Errors);
        }

        #endregion

        #region UpdateProducto - PUT /api/producto/{id}

        [Fact]
        public async Task UpdateProducto_DebeRetornar200_CuandoActualizacionExitosa()
        {
            // Arrange
            var updateDto = new ProductoUpdateDto
            {
                Nombre = "Laptop Actualizada",
                Descripcion = "Laptop Dell XPS actualizada",
                Precio = 18000m
            };

            var productoActualizado = new ProductoDto
            {
                Id = 1,
                Nombre = "Laptop Actualizada",
                Descripcion = "Laptop Dell XPS actualizada",
                Precio = 18000m
            };

            _mockProductoService.Setup(s => s.UpdateProductoAsync(1, updateDto))
                .ReturnsAsync(productoActualizado);

            // Act
            var resultado = await _controller.UpdateProducto(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<ProductoDto>>(okResult.Value);

            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(18000m, response.Data.Precio);
        }

        [Fact]
        public async Task UpdateProducto_DebeRetornar404_CuandoProductoNoExiste()
        {
            // Arrange
            var updateDto = new ProductoUpdateDto
            {
                Nombre = "Producto",
                Descripcion = "Descripción",
                Precio = 100m
            };

            _mockProductoService.Setup(s => s.UpdateProductoAsync(999, updateDto))
                .ThrowsAsync(new KeyNotFoundException("Producto no encontrado"));

            // Act
            var resultado = await _controller.UpdateProducto(999, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<ProductoDto>>(notFoundResult.Value);
            Assert.False(response.Success);
        }

        #endregion

        #region DeleteProducto - DELETE /api/producto/{id}

        [Fact]
        public async Task DeleteProducto_DebeRetornar200_CuandoEliminacionExitosa()
        {
            // Arrange
            _mockProductoService.Setup(s => s.DeleteProductoAsync(1))
                .ReturnsAsync(true);

            // Act
            var resultado = await _controller.DeleteProducto(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);

            Assert.True(response.Success);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task DeleteProducto_DebeRetornar404_CuandoProductoNoExiste()
        {
            // Arrange
            _mockProductoService.Setup(s => s.DeleteProductoAsync(999))
                .ReturnsAsync(false);

            // Act
            var resultado = await _controller.DeleteProducto(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
            Assert.False(response.Success);
        }

        #endregion

        #region SearchProductos - GET /api/producto/search

        [Fact]
        public async Task SearchProductos_DebeRetornarProductosCoincidentes()
        {
            // Arrange
            var productosEncontrados = new List<ProductoDto>
            {
                new ProductoDto { Id = 1, Nombre = "Laptop HP", Descripcion = "HP Pavilion", Precio = 15000m },
                new ProductoDto { Id = 2, Nombre = "Laptop Dell", Descripcion = "Dell XPS", Precio = 20000m }
            };

            _mockProductoService.Setup(s => s.SearchProductosByNombreAsync("Laptop"))
                .ReturnsAsync(productosEncontrados);

            // Act
            var resultado = await _controller.SearchProductos("Laptop");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProductoDto>>>(okResult.Value);

            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.Data.Count());
        }

        [Fact]
        public async Task SearchProductos_DebeRetornarListaVacia_SinCoincidencias()
        {
            // Arrange
            _mockProductoService.Setup(s => s.SearchProductosByNombreAsync("NoExiste"))
                .ReturnsAsync(new List<ProductoDto>());

            // Act
            var resultado = await _controller.SearchProductos("NoExiste");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProductoDto>>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Empty(response.Data);
        }

        #endregion

        #region GetProductosByPrecioRange - GET /api/producto/precio-range

        [Fact]
        public async Task GetProductosByPrecioRange_DebeRetornar200_ConRangoValido()
        {
            // Arrange
            var productos = new List<ProductoDto>
            {
                new ProductoDto { Id = 1, Nombre = "Mouse", Descripcion = "Desc", Precio = 1500m },
                new ProductoDto { Id = 2, Nombre = "Teclado", Descripcion = "Desc", Precio = 2500m }
            };

            _mockProductoService.Setup(s => s.GetProductosByPrecioRangeAsync(1000m, 5000m))
                .ReturnsAsync(productos);

            // Act
            var resultado = await _controller.GetProductosByPrecioRange(1000m, 5000m);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProductoDto>>>(okResult.Value);

            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.Data.Count());
        }

        [Fact]
        public async Task GetProductosByPrecioRange_DebeRetornar400_ConRangoInvalido()
        {
            // Act
            var resultado = await _controller.GetProductosByPrecioRange(5000m, 1000m);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProductoDto>>>(badRequestResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Rango de precios inválido", response.Message);
        }

        #endregion
    }
}
