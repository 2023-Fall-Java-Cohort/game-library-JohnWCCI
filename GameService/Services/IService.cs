using System.Linq.Expressions;

namespace GameService.Services
{
    public interface IService<TEntity> where TEntity : class
    {
       Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<TEntity?> GetAsync(int id, CancellationToken cancellationToken = default(CancellationToken));
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TEntity>> GetAllPageAsync(int pageIndex, int pageSize = 10, string orderBy = "Id", CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TEntity>> GetSearchEntityAsync( Expression<Func<TEntity, bool>> predicate, int PageIndex= 1, int PageSize = 20, string OrderByColumn = "Name" , CancellationToken cancellationToken = default(CancellationToken));
    }
}
