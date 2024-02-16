using GameService.Services;

namespace GameService
{
    public static class Startup
    {

        public static IServiceCollection AddDbService(this IServiceCollection services)
        {
            services.AddDbContext<GameContext>();
            services.AddScoped<IBoardGameService, BoardGameService>();
            services.AddScoped<IPublisherService, PublisherService>();
            return services;
        }
    }
}
