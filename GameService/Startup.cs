using GameService.Services;

namespace GameService
{
    public static class Startup
    {

        public static IServiceCollection AddDbService(this IServiceCollection services)
        {
            services.AddDbContext<GameContext>();
            services.AddTransient<IBoardGameService, BoardGameService>();
            services.AddTransient<IPublisherService, PublisherService>();
            return services;
        }
    }
}
