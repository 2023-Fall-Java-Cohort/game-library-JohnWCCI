using GameDataLibrary;
using Microsoft.EntityFrameworkCore;

namespace GameService.Services
{
    public class PublisherService : Service<PublisherModel>, IPublisherService
    {
        public PublisherService(GameContext dbContext, ILogger<PublisherService> logger) :
            base(dbContext, logger)
        { }
        
    }
}
