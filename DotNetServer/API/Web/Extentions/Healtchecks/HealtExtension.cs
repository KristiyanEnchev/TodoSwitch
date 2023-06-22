namespace Web.Extentions.Healtchecks
{
    using Application.Interfaces.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Caching.Memory;
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
            healthChecks.AddCheck<CacheHealthCheck>("cache_health_check");
            healthChecks.AddCheck("disk_space_health_check",
            new DiskSpaceHealthCheck(minimumFreeDiskSpace: 10L * 1024L * 1024L * 1024L, driveName: "C:\\"));
            healthChecks.AddCheck("memory_health_check",
            new MemoryHealthCheck(maxAllowedMemory: 1024L * 1024L * 1024L));

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

        public class CacheHealthCheck : IHealthCheck
        {
            private readonly IMemoryCache _cache;
            private readonly string _testCacheKey = "health_check_cache_key";
            private readonly string _testCacheValue = "test_value";

            public CacheHealthCheck(IMemoryCache cache)
            {
                _cache = cache;
            }

            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                try
                {
                    _cache.Set(_testCacheKey, _testCacheValue);

                    if (_cache.TryGetValue(_testCacheKey, out string cachedValue))
                    {
                        if (cachedValue == _testCacheValue)
                        {
                            return Task.FromResult(HealthCheckResult.Healthy("Cache is healthy"));
                        }
                    }

                    return Task.FromResult(HealthCheckResult.Unhealthy("Cache is not working as expected"));
                }
                catch (Exception ex)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("Cache health check failed", ex));
                }
            }
        }

        public class DiskSpaceHealthCheck : IHealthCheck
        {
            private readonly long _minimumFreeDiskSpace;
            private readonly string _driveName;

            public DiskSpaceHealthCheck(long minimumFreeDiskSpace, string driveName)
            {
                _minimumFreeDiskSpace = minimumFreeDiskSpace;
                _driveName = driveName;
            }

            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                try
                {
                    var drive = DriveInfo.GetDrives().FirstOrDefault(d => d.Name.Equals(_driveName, StringComparison.OrdinalIgnoreCase));
                    if (drive == null)
                    {
                        return Task.FromResult(HealthCheckResult.Unhealthy($"Drive '{_driveName}' not found"));
                    }

                    if (drive.AvailableFreeSpace >= _minimumFreeDiskSpace)
                    {
                        return Task.FromResult(HealthCheckResult.Healthy("Sufficient disk space available"));
                    }

                    return Task.FromResult(HealthCheckResult.Unhealthy($"Insufficient disk space. Available: {drive.AvailableFreeSpace / 1024 / 1024} MB"));
                }
                catch (Exception ex)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("Disk space health check failed", ex));
                }
            }
        }

        public class MemoryHealthCheck : IHealthCheck
        {
            private readonly long _maxAllowedMemory;

            public MemoryHealthCheck(long maxAllowedMemory)
            {
                _maxAllowedMemory = maxAllowedMemory;
            }

            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                var memoryUsed = GC.GetTotalMemory(false);

                if (memoryUsed <= _maxAllowedMemory)
                {
                    return Task.FromResult(HealthCheckResult.Healthy("Memory usage is within limits"));
                }

                return Task.FromResult(HealthCheckResult.Unhealthy($"Memory usage is too high. Current usage: {memoryUsed / 1024 / 1024} MB"));
            }
        }

        internal static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
            endpoints.MapHealthChecks("/api/health", new HealthCheckOptions
            {
                ResponseWriter = (httpContext, result) => CustomHealthCheckResponseWriter.WriteResponse(httpContext, result),
            });
    }
}