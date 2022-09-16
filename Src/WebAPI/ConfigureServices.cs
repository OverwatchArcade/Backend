using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Persistence;
using WebAPI.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUiServices(this IServiceCollection services, IConfiguration configuration)
    {
        Other(services);
        ConfigureSwagger(services);
        ConfigureCorsPolicy(services);
        ConfigureAuthentication(services, configuration);

        return services;
    }

    // public static void AddFactories(IServiceCollection serviceCollection)
    // {
    //     serviceCollection
    //         .AddScoped<IDailyFactory, DailyFactory>()
    //         .AddScoped(typeof(IServiceResponseFactory<>), typeof(ServiceResponseFactory<>))
    //
    //         .AddScoped<ITwitterClientFactory, TwitterClientFactory>();
    // }


    private static void Other(IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpContextAccessor();

        serviceCollection.AddSingleton<ICurrentUserService, CurrentUserService>();
        serviceCollection.AddSingleton<ICacheWarmupService, CacheWarmupService>();
        serviceCollection.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();

        serviceCollection
            .AddSingleton<IMemoryCache, MemoryCache>();
    }

    private static void ConfigureSwagger(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v2",
                Title = "OverwatchArcade.Today API",
                Description =
                    "An ASP.NET Core Web API for retrieving information about Overwatch Arcade such as the daily arcademodes.",
                License = new OpenApiLicense()
                {
                    Name = "Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International",
                    Url = new Uri("https://github.com/OverwatchArcade/Backend/blob/main/LICENSE.md")
                }
            });
        });
    }

    private static void ConfigureCorsPolicy(IServiceCollection serviceCollection)
    {
        serviceCollection.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder => builder
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins("https://*.overwatcharcade.today", "https://overwatcharcade.today",
                        "https://*.owfrontend.pages.dev", "http://localhost:3000")
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

    private static void ConfigureAuthentication(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                        .GetBytes(configuration.GetValue<string>("Jwt:Token"))),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
    }
}