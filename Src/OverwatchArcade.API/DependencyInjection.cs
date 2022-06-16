using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.API.Services.AuthService;
using OverwatchArcade.API.Services.CachingService;
using OverwatchArcade.API.Services.ConfigService;
using OverwatchArcade.API.Services.ContributorService;
using OverwatchArcade.API.Services.OverwatchService;
using OverwatchArcade.API.Validators;
using OverwatchArcade.API.Validators.Overwatch;
using OverwatchArcade.Domain.Factories;
using OverwatchArcade.Domain.Factories.interfaces;
using OverwatchArcade.Persistence;
using OverwatchArcade.Persistence.Repositories;
using OverwatchArcade.Persistence.Repositories.Interfaces;
using OWArcadeBackend.Services.CachingService;

namespace OverwatchArcade.API;

public class DependencyInjection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IConfigService, ConfigService>()
            .AddScoped<IOverwatchService, OverwatchService>()
            .AddScoped<ICacheWarmupService, CacheWarmupService>()
            .AddScoped<IContributorService, ContributorService>();
    }

    public static void AddFactories(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IDailyFactory, DailyFactory>();
    }
    
    public static void AddValidators(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IValidator<CreateDailyDto>, CreateDailyDtoValidator>()
            .AddScoped<IValidator<CreateTileModeDto>, CreateTileModesDtoValidator>();
    }
    public static void AddRepositories(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IDailyRepository, DailyRepository>()
            .AddScoped<IAuthRepository, AuthRepository>()
            .AddScoped<IConfigRepository, ConfigRepository>()
            .AddScoped<IOverwatchRepository, OverwatchRepository>()
            .AddScoped<ILabelRepository, LabelRepository>()
            .AddScoped<IContributorRepository, ContributorRepository>()
            .AddScoped<IWhitelistRepository, WhitelistRepository>()
            .AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
    
    public static void Other(IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient();
        serviceCollection
            .AddSingleton<IMemoryCache, MemoryCache>();
    }
    
    public static void ConfigureCorsPolicy(IServiceCollection serviceCollection)
    {
        serviceCollection.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder => builder
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins("https://*.overwatcharcade.today", "https://overwatcharcade.today", "https://*.owfrontend.pages.dev", "http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .Build()
            );
            options.AddPolicy("OpenAPI",
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }
}