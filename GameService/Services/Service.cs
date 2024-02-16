using GameDataLibrary;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GameService.Services
{
    /// <summary>
    /// Main service code
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="GameService.Services.IService&lt;TEntity&gt;" />
    public abstract class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        private readonly GameContext dbContext;
        private readonly ILogger logger;
        private readonly string? className;
        public Service(GameContext dbContext, ILogger logger)
        {
            this.dbContext = dbContext ?? throw new System.ArgumentNullException(nameof(dbContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.className = typeof(TEntity).FullName;
        }


        /// <summary>
        /// Adds the entity to the database asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
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

        /// <summary>
        /// Adds the range of entities to the database asynchronous.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes the entity from the database asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
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

        /// <summary>
        /// filter entities based on the condition provided in the lambda expression.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets all entities from the database asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets all entities using pagination asynchronous.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a single entity based on Id asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
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

        /// <summary>
        /// filter entities based on the condition provided in the lambda expression.
        /// and uses pagination on the return 
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="OrderByColumn">The order by column.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Updates an entity asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
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
