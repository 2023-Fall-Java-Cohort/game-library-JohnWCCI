using GameDataLibrary;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GameService.Services
{
    public abstract class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        private readonly DbContext dbContext;
        private readonly ILogger logger;
        private readonly string? className;
        public Service(DbContext dbContext, ILogger logger)
        {
            this.dbContext = dbContext ?? throw new System.ArgumentNullException(nameof(dbContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.className = typeof(TEntity).FullName;
        }

       

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                await dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                dbContext.Entry(entity).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to add {className} Entity to database. {entity}");
                throw;
            }
            return entity;
        }

        public virtual async Task<int> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            int result = 0;
            try
            {
                await dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
                result = await dbContext.SaveChangesAsync( cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to addRange {className} Entities to database.");
                throw;
            }
            return result;
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            bool result = false;
            try
            {
                _ = dbContext.Set<TEntity>().Remove(entity);
               result = ( await dbContext.SaveChangesAsync(cancellationToken) == 1);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to Delete {className} Entity to database. {entity}");
                throw;
            }
            return result;
        }

        public virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                IQueryable<TEntity> query = dbContext.Set<TEntity>().Where(predicate);
                return await query.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to Find {className} Entity in database. {predicate}");
                throw;
            }
            
        }

        public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await dbContext.Set<TEntity>()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error getting all {className} from the Database");
                throw;
            }
        }

        public virtual async Task<List<TEntity>> GetAllPageAsync(int pageIndex, int pageSize = 10, string orderBy = "Id", CancellationToken cancellationToken = default)
        {
            try
            {
                IQueryable<TEntity> entities = dbContext.Set<TEntity>()
                    .AsNoTracking()
                    .OrderBy(o => EF.Property<object>(o, orderBy))
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);
                return await entities.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to Get by page {className} Entities from database. page index {pageIndex}, page size {pageSize}");
                throw;
            }
        }

        public virtual async Task<TEntity?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            TEntity? result = null;
            try
            {
                IQueryable<TEntity> entities = dbContext.Set<TEntity>()
                    .AsNoTracking()
                    .Where(w => EF.Property<int>(w, "id") == id);

               result = await entities.FirstOrDefaultAsync(cancellationToken);
                if(result is null)
                {
                    throw new Exception($"{className} {id} was not found in the table");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to Get {className} Entity with id {id} from database.");
                throw;
            }
            return result;
        }

        public virtual async Task<List<TEntity>> GetSearchEntityAsync(Expression<Func<TEntity, bool>> predicate, int pageIndex = 1, int pageSize = 20, string OrderByColumn = "Name", CancellationToken cancellationToken = default)
        {
            try
            {
                IQueryable<TEntity> entities = dbContext.Set<TEntity>()
                   .AsNoTracking()
                   .Where(predicate)
                   .OrderBy(o => EF.Property<object>(o, OrderByColumn))
                   .Skip(pageIndex * pageSize)
                   .Take(pageSize);
                return await entities.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to Search by page {className} Entities from database. page index {pageIndex}, page size {pageSize}");
                throw;
            }
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                dbContext.Set<TEntity>().Add(entity).State = EntityState.Modified;
                await dbContext.SaveChangesAsync(cancellationToken);
                dbContext.Entry(entity).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to add {className} Entity to database. {entity}");
                throw;
            }
            return entity;
        }
    }
}
