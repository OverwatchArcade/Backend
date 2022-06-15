using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OverwatchArcade.API;
using OverwatchArcade.Persistence;
using OWArcadeBackend.Services.CachingService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
DependencyInjection.Other(builder.Services);
DependencyInjection.AddServices(builder.Services);
DependencyInjection.AddFactories(builder.Services);
DependencyInjection.AddValidators(builder.Services);
DependencyInjection.AddRepositories(builder.Services);
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

builder.Services.AddDbContextPool<AppDbContext>(opt =>opt.UseSqlServer(builder.Configuration["Database:OWarcade"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = ((IApplicationBuilder)app) .ApplicationServices.CreateScope())
{
    await using (var context = scope.ServiceProvider.GetService<AppDbContext>())
    {
        context?.Database.Migrate();
    }
    var cacheWarmupService = scope.ServiceProvider.GetService<ICacheWarmupService>();
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