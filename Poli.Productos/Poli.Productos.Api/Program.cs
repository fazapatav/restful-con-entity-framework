using Microsoft.EntityFrameworkCore;
using Poli.Productos.Api.Middleware;
using Poli.Productos.Aplication.Interfaces;
using Poli.Productos.Aplication.UsesCase;
using Poli.Productos.Domain.Interfaces;
using Poli.Productos.Infrastructure.Data;
using Poli.Productos.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configuración de DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<Poli.Productos.Aplication.Mappings.MappingProfile>();
});

// Registro de Repositorios y Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

// Registro de Servicios de Aplicación
builder.Services.AddScoped<IProductoService, ProductoService>();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Poli Productos API",
        Version = "v1",
        Description = "API RESTful para gestión de productos con arquitectura limpia",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Poli Productos",
            Email = "contact@poliproductos.com"
        }
    });
});

var app = builder.Build();

// Aplicar migraciones automáticamente al iniciar la aplicación (Code First)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Esto creará la base de datos si no existe y aplicará migraciones pendientes
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al aplicar las migraciones de la base de datos.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Poli Productos API v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz (http://localhost:5125/)
    });
    app.UseDeveloperExceptionPage();
}

// Middleware personalizado de manejo de excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
