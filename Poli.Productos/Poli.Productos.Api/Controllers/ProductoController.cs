using Microsoft.AspNetCore.Mvc;
using Poli.Productos.Aplication.Dtos;

namespace Poli.Productos.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductoController : ControllerBase
    {
        [HttpGet(Name = "GetProductos")]
        public IEnumerable<ProductoDto> Get()
        {
            return new List<ProductoDto> { };
        }
    }
}
