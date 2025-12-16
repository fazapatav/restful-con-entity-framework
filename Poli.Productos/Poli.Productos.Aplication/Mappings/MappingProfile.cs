using AutoMapper;
using Poli.Productos.Aplication.Dtos;
using Poli.Productos.Domain.Entities;

namespace Poli.Productos.Aplication.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Producto Entity <-> DTOs
            CreateMap<Producto, ProductoDto>();
            CreateMap<ProductoCreateDto, Producto>();
            CreateMap<ProductoUpdateDto, Producto>();
        }
    }
}
