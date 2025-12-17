using Microsoft.AspNetCore.Mvc;
using Poli.Productos.Aplication.Common;
using Poli.Productos.Aplication.Dtos;
using Poli.Productos.Aplication.Interfaces;

namespace Poli.Productos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductoController> _logger;

        public ProductoController(IProductoService productoService, ILogger<ProductoController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductoDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductoDto>>>> GetAllProductos()
        {
            var productos = await _productoService.GetAllProductosAsync();
            return Ok(ApiResponse<IEnumerable<ProductoDto>>.SuccessResponse(productos, "Productos obtenidos exitosamente"));
        }

        /// <summary>
        /// Obtiene un producto por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProductoDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductoDto>>> GetProductoById(int id)
        {
            var producto = await _productoService.GetProductoByIdAsync(id);

            if (producto == null)
            {
                return NotFound(ApiResponse<ProductoDto>.ErrorResponse($"Producto con ID {id} no encontrado"));
            }

            return Ok(ApiResponse<ProductoDto>.SuccessResponse(producto, "Producto encontrado"));
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductoDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<ProductoDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ProductoDto>>> CreateProducto([FromBody] ProductoCreateDto productoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ProductoDto>.ErrorResponse("Datos inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var producto = await _productoService.CreateProductoAsync(productoDto);
            return CreatedAtAction(nameof(GetProductoById), new { id = producto.Id },
                ApiResponse<ProductoDto>.SuccessResponse(producto, "Producto creado exitosamente"));
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProductoDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ProductoDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductoDto>>> UpdateProducto(int id, [FromBody] ProductoUpdateDto productoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ProductoDto>.ErrorResponse("Datos inválidos",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            try
            {
                var producto = await _productoService.UpdateProductoAsync(id, productoDto);
                return Ok(ApiResponse<ProductoDto>.SuccessResponse(producto, "Producto actualizado exitosamente"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<ProductoDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Elimina un producto
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProducto(int id)
        {
            var deleted = await _productoService.DeleteProductoAsync(id);

            if (!deleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse($"Producto con ID {id} no encontrado"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Producto eliminado exitosamente"));
        }

        /// <summary>
        /// Busca productos por nombre
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductoDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductoDto>>>> SearchProductos([FromQuery] string nombre)
        {
            var productos = await _productoService.SearchProductosByNombreAsync(nombre);
            return Ok(ApiResponse<IEnumerable<ProductoDto>>.SuccessResponse(productos,
                $"Se encontraron {productos.Count()} productos"));
        }

        /// <summary>
        /// Obtiene productos por rango de precio
        /// </summary>
        [HttpGet("precio-range")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductoDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductoDto>>>> GetProductosByPrecioRange(
            [FromQuery] decimal minPrecio,
            [FromQuery] decimal maxPrecio)
        {
            if (minPrecio < 0 || maxPrecio < 0 || minPrecio > maxPrecio)
            {
                return BadRequest(ApiResponse<IEnumerable<ProductoDto>>.ErrorResponse(
                    "Rango de precios inválido"));
            }

            var productos = await _productoService.GetProductosByPrecioRangeAsync(minPrecio, maxPrecio);
            return Ok(ApiResponse<IEnumerable<ProductoDto>>.SuccessResponse(productos,
                $"Se encontraron {productos.Count()} productos en el rango"));
        }
    }
}
