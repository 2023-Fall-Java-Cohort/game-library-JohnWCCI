using GameDataLibrary;

namespace GameLibrary.Repositories
{
    public class PublisherRepository : Repository<PublisherModel>, IPublisherRepository
    {
        public PublisherRepository(ILogger<Repository<PublisherRepository>> logger, HttpClient httpClient, IConfiguration configuration) 
            : base(logger, httpClient, configuration)  
        {
            
        }
    }
}
