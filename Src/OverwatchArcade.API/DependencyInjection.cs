using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Dtos.Discord;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.API.Factories;
using OverwatchArcade.API.Factories.Interfaces;
using OverwatchArcade.API.Services;
using OverwatchArcade.API.Services.AuthService;
using OverwatchArcade.API.Services.ConfigService;
using OverwatchArcade.API.Services.ContributorService;
using OverwatchArcade.API.Services.OverwatchService;
using OverwatchArcade.API.Utility;
using OverwatchArcade.API.Utility.Interfaces;
using OverwatchArcade.API.Validators;
using OverwatchArcade.API.Validators.Contributor;
using OverwatchArcade.API.Validators.Overwatch;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Domain.Factories;
using OverwatchArcade.Domain.Factories.interfaces;
using OverwatchArcade.Persistence;
using OverwatchArcade.Persistence.Repositories;
using OverwatchArcade.Persistence.Repositories.Interfaces;
using OverwatchArcade.Twitter.Factories;
using OverwatchArcade.Twitter.Services.ScreenshotService;
using OverwatchArcade.Twitter.Services.TwitterService;
using IFileProvider = OverwatchArcade.API.Utility.Interfaces.IFileProvider;

namespace Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    
    // public static void AddFactories(IServiceCollection serviceCollection)
    // {
    //     serviceCollection
    //         .AddScoped<IDailyFactory, DailyFactory>()
    //         .AddScoped(typeof(IServiceResponseFactory<>), typeof(ServiceResponseFactory<>))
    //
    //         .AddScoped<ITwitterClientFactory, TwitterClientFactory>();
    // }
    
    
    public static void Other(IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient();
        serviceCollection.AddHttpContextAccessor();
        
        serviceCollection.AddSingleton<ICurrentUserService, CurrentUserService>();
        serviceCollection.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();

        serviceCollection
            .AddScoped<IFileProvider, FileProvider>()
            .AddScoped<IDiscordApiClient, DiscordApiClient>()
            .AddScoped<IAuthenticationToken, AuthenticationToken>()
            .AddSingleton<IMemoryCache, MemoryCache>();
    }
    
    public static void ConfigureSwagger(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v2",
                Title = "OverwatchArcade.Today API",
                Description = "An ASP.NET Core Web API for retrieving information about Overwatch Arcade such as the daily arcademodes.",
                License = new OpenApiLicense()
                {
                    Name = "Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International",
                    Url = new Uri("https://github.com/OverwatchArcade/Backend/blob/main/LICENSE.md")
                }
            });
        });
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