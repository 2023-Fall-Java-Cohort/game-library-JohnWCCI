using GameDataLibrary;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace GameService.Services
{
    /// <summary>
    /// Handles publisher service
    /// </summary>
    /// <seealso cref="GameService.Services.Service&lt;GameDataLibrary.PublisherModel&gt;" />
    /// <seealso cref="GameService.Services.IPublisherService" />
    public class PublisherService : Service<PublisherModel>, IPublisherService
    {
        private readonly GameContext dbContext;
        private readonly ILogger<PublisherService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        public PublisherService(GameContext dbContext, ILogger<PublisherService> logger) :
            base(dbContext, logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        /// <summary>
        /// Gets a single entity based on Id asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public override async Task<PublisherModel?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
           
            try
            {
                IQueryable<PublisherModel> entities = dbContext.Publishers
                    .AsNoTracking()
                    .Include(bg => bg.BoardGames)
                    .Where(w => w.Id == id);

              return await entities.FirstOrDefaultAsync(cancellationToken);
              
            }
            catch (Exception ex)
            {

                logger.LogError(ex, $"Failed to Get PublisherModel Entity with id {id} from database.");
                throw;
            }
        }
    }
}
