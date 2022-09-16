using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DeßpendencyInjection;
using NLog;
using NLog.Web;
using OverwatchArcade.Application.Cache.Commands.Warmup;
using OverwatchArcade.Application.Common.Exceptions;
using OverwatchArcade.Application.Common.Interfaces;
using OverwatchArcade.Persistence;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddWebUiServices(builder.Configuration);
    
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    using var scope = ((IApplicationBuilder)app).ApplicationServices.CreateScope();
    var mediatr = scope.ServiceProvider.GetService<IMediator>();
    await mediatr.Send(new CacheWarmupCommand());

    // await using (var context = scope.ServiceProvider.GetService<ApplicationDbContext>())
    // {
    //     await context.Database.MigrateAsync();
    // }


    
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