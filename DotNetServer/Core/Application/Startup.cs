namespace Application
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    public static class Startup
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddMediator();
        }

        private static void AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
        }
    }
}
