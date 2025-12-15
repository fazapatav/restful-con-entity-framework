using System;
using System.Collections.Generic;
using System.Text;

namespace Poli.Productos.Domain.Entities
{
    public class Producto
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public decimal Precio { get; set; }
    }
}
