namespace Persistence
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using MongoDB.Driver;

    using Persistence.Initializers;
    using Persistence.Repository;

    public static class Startup
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddMongoClient(configuration)
                .AddRepositories();

            return services;
        }

        private static IServiceCollection AddMongoClient(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(connectionString));
            services.AddScoped<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase("TodoSwitch");
            });

            services.AddScoped<DbInitializer>();
            services.AddScoped<DbSeeder>();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services
                .AddTransient(typeof(IMongoRepository<>), typeof(MongoRepository<>));

            return services;
        }
    }
}