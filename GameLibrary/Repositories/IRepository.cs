namespace GameLibrary.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        ValueTask<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
        ValueTask<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
        ValueTask<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));
        ValueTask<TEntity> GetAsync(int id, CancellationToken cancellationToken = default(CancellationToken));
        ValueTask<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
    }
}
