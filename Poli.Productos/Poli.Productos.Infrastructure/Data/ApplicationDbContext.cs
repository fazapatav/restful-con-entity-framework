using Microsoft.EntityFrameworkCore;
using Poli.Productos.Domain.Entities;

namespace Poli.Productos.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Precio)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                // Datos semilla opcionales
                entity.HasData(
                    new Producto { Id = 1, Nombre = "Laptop", Descripcion = "Laptop de alta gama", Precio = 1200.00m },
                    new Producto { Id = 2, Nombre = "Mouse", Descripcion = "Mouse inalámbrico", Precio = 25.99m },
                    new Producto { Id = 3, Nombre = "Teclado", Descripcion = "Teclado mecánico", Precio = 89.99m }
                );
            });
        }
    }
}
