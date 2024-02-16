using GameLibrary.Repositories;

namespace GameLibrary
{
    public static class Startup
    {
        public static IServiceCollection AddApiService(this IServiceCollection service)
        {
            service.AddHttpClient<PublisherRepository>();
           // service.AddHttpClient<BoardGameRepository>();
            service.AddTransient<IPublisherRepository, PublisherRepository>();
            //service.AddTransient<IBoardGameRepository, BoardGameRepository>();
            return service;
        }
    }
}
