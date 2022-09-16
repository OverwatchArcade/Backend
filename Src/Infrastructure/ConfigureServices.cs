using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Application.Common.Utility;
using OverwatchArcade.Persistence;
using OverwatchArcade.Persistence.ApiClient;
using OverwatchArcade.Persistence.ApiClient.Interfaces;
using OverwatchArcade.Persistence.Services;
using OverwatchArcade.Persistence.Services.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetValue<string>("Database:OWArcade"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddTransient<IFileProvider, FileProvider>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IDiscordClient, DiscordClient>();

        services.AddHttpClient();
        

        return services;
    }
}