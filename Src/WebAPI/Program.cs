using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Persistence;
using WebAPI.Services;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    builder.Services.AddApplicationServices();
    
    DependencyInjection.Other(builder.Services);
    DependencyInjection.ConfigureSwagger(builder.Services);
    DependencyInjection.ConfigureCorsPolicy(builder.Services);

    builder.Services.AddAuthentication(option =>
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
                    .GetBytes(builder.Configuration.GetValue<string>("Jwt:Token"))),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

    builder.Services.AddDbContextPool<ApplicationDbContext>(opt => opt.UseSqlServer(builder.Configuration["Database:OWArcade"]));

    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    using (var scope = ((IApplicationBuilder)app).ApplicationServices.CreateScope())
    {
        await using (var context = scope.ServiceProvider.GetService<ApplicationDbContext>())
        {
            context?.Database.Migrate();
        }

        var cacheWarmupService = scope.ServiceProvider.GetService<ICacheWarmupService>();
        if (cacheWarmupService == null)
        {
            throw new ArgumentNullException(nameof(CacheWarmupService), "Can't run cache warmup service");
        }
        
        await cacheWarmupService.Run();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseAuthentication();

    app.UseAuthorization();

    app.UseCors();

    app.UseResponseCaching();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}