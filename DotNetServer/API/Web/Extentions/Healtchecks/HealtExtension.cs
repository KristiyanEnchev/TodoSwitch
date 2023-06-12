namespace Web.Extentions.Healtchecks
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    using Models.HealthCheck;

    internal static class HealtExtension
    {
        internal static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration)
        {
            var healthSettings = configuration.GetSection(nameof(Health)).Get<Health>();

            services.AddSingleton<CustomHealthCheckResponseWriter>();

            var databaseHealthChecks = healthSettings?.DatabaseHealthChecks;

            var healthChecks = services.AddHealthChecks();

            if (databaseHealthChecks != null && (bool)databaseHealthChecks)
            {
                healthChecks.AddMongoDb(configuration.GetConnectionString("DefaultConnection")!);
            }

            healthChecks.AddCheck<ControllerHealthCheck>("controller_health_check");

            return services;
        }

        public class ControllerHealthCheck : IHealthCheck
        {
            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                bool isControllerHealthy = true;

                if (isControllerHealthy)
                {
                    return Task.FromResult(HealthCheckResult.Healthy("Controller is healthy"));
                }

                return Task.FromResult(HealthCheckResult.Unhealthy("Controller is unhealthy"));
            }
        }

        internal static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
            endpoints.MapHealthChecks("/api/health", new HealthCheckOptions
            {
                ResponseWriter = (httpContext, result) => CustomHealthCheckResponseWriter.WriteResponse(httpContext, result),
            });
    }
}