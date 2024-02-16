
using static System.Collections.Specialized.BitVector32;

namespace GameLibrary.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string className;
        private readonly string baseAddress;

        public Repository(ILogger logger, HttpClient httpClient, IConfiguration configuration) {
            this.logger = logger;
            this.httpClient = httpClient;
            this.configuration = configuration;

            IConfigurationSection section = configuration.GetSection("ServiceURL") ?? 
                throw new ArgumentException("Service URL not defined in AppSettings.json");

            this.className = typeof(TEntity).FullName ?? "TEntity";

            this.baseAddress = section.GetValue<string>(typeof(TEntity).Name) ?? 
                throw new System.ArgumentNullException($"Service path {typeof(TEntity).Name} is not defined in AppSettings.json");
        }

        public async ValueTask<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            List<TEntity> result = new List<TEntity>();
            try
            {
                HttpResponseMessage? response = await httpClient.GetAsync($"{baseAddress}");
                result = await response.ReadContentAsync<List<TEntity>>(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error");
             
            }
            return result;
        }


        public ValueTask<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
           

        }

        public ValueTask<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

      
        public ValueTask<TEntity> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

     
    }
}
