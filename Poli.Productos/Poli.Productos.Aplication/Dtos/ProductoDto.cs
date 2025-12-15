using System;
using System.Collections.Generic;
using System.Text;

namespace Poli.Productos.Aplication.Dtos
{
    public class ProductoDto
    {
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public decimal Precio { get; set; }
    }
}
