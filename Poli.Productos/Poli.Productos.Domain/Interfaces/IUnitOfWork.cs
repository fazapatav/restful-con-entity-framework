namespace Poli.Productos.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductoRepository Productos { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
