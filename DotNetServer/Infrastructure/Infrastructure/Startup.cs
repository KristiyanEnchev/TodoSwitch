namespace Infrastructure
{
    using System.Text;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using MediatR;

    using Domain.Entities;

    using Models;

    using Infrastructure.Services.Identity;
    using Infrastructure.Services.Token;
    using Infrastructure.Services.Todo;
    using Infrastructure.Services.Background;

    using Application.Interfaces.Services;

    using Persistence.Initializers;

    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddServices()
                .AddIdentity(configuration)
                .AddConfigurations(configuration)
                .AddCustomAuthentiation(configuration);

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();
            services.AddScoped<ITodoBackgroundJobService, TodoBackgroundJobService>();

            services.AddTransient<ITodoService, TodoService>();
            services.AddTransient<IMediator, Mediator>();

            services.AddScoped<ICachedTodoService, CachedTodoService>();

            return services;
        }

        public static async Task InitializeDatabase(this IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<DbInitializer>();
            var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();

            await initialiser.Initialize();

            await seeder.SeedData();
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services
                .AddTransient<IIdentity, IdentityService>()
                .AddTransient<IJwtService, JwtService>()
                .AddIdentity<User, UserRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddMongoDbStores<User, UserRole, string>
                (
                    connectionString, "TodoSwitch"
                )
                .AddDefaultTokenProviders()
                .AddTokenProvider("TodoSwitch", typeof(DataProtectorTokenProvider<User>));

            return services;
        }

        public static IServiceCollection AddCustomAuthentiation(this IServiceCollection services, IConfiguration configuration)
        {
            var key = configuration.GetSection(nameof(TokenSettings)).GetValue<string>(nameof(TokenSettings.Key))!;
            var audience = configuration.GetSection(nameof(TokenSettings)).GetValue<string>(nameof(TokenSettings.Audience))!;
            var issuer = configuration.GetSection(nameof(TokenSettings)).GetValue<string>(nameof(TokenSettings.Issuer))!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

            return services;
        }

        private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenSettings>(configuration.GetSection(nameof(TokenSettings)));

            return services;
        }
    }
}