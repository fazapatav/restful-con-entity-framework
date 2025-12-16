namespace Poli.Productos.Aplication.Dtos
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public decimal Precio { get; set; }
    }
}
