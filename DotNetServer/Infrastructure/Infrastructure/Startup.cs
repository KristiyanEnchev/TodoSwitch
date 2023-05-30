namespace Infrastructure
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using MediatR;

    using Persistence;

    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddServices();

            services.AddPersistence(configuration);

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddTransient<IMediator, Mediator>();

            return services;
        }
    }
}