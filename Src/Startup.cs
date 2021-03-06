using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OWArcadeBackend.Persistence;
using OWArcadeBackend.Persistence.Repositories;
using OWArcadeBackend.Persistence.Repositories.Interfaces;
using OWArcadeBackend.Services.ContributorService;
using OWArcadeBackend.Services.OverwatchService;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using OWArcadeBackend.Factories;
using OWArcadeBackend.Services.AuthService;
using OWArcadeBackend.Services.CachingService;
using OWArcadeBackend.Services.ConfigService;
using OWArcadeBackend.Services.TwitterService;

namespace OWArcadeBackend
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDatabaseServices(services);

            services.AddControllers()
                .AddNewtonsoftJson()
                .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

            services.AddAutoMapper(typeof(Startup));

            services.AddResponseCaching();

            services.AddAuthentication(option =>
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
                            .GetBytes(Configuration.GetSection("Jwt:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            ConfigureCorsPolicy(services);
            
            ConfigureOtherDependencyInjection(services);
            ConfigureFactoryDependencyInjection(services);
            ConfigureServicesDependencyInjection(services);
            ConfigureRepositoryDependencyInjection(services);
        }
        
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<AppDbContext>())
                {
                    context?.Database.Migrate();
                }
                var cacheWarmupService = scope.ServiceProvider.GetService<ICacheWarmupService>();
                BackgroundJob.Enqueue(() => cacheWarmupService.Run());
            }
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();

            app.UseStaticFiles();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseHangfireDashboard();

            app.UseResponseCaching();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static void ConfigureOtherDependencyInjection(IServiceCollection services)
        {
            services.AddHttpClient();

            services
                .AddSingleton<IMemoryCache, MemoryCache>();
        }

        private static void ConfigureServicesDependencyInjection(IServiceCollection services)
        {
            services
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IConfigService, ConfigService>()
                .AddScoped<ITwitterService, TwitterService>()
                .AddScoped<IOverwatchService, OverwatchService>()
                .AddScoped<ICacheWarmupService, CacheWarmupService>()
                .AddScoped<IContributorService, ContributorService>();
        }

        private static void ConfigureRepositoryDependencyInjection(IServiceCollection services)
        {
            services
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
        
        private static void ConfigureFactoryDependencyInjection(IServiceCollection services)
        {
            services.AddScoped<ITwitterClientFactory, TwitterClientFactory>();
        }

        private static void ConfigureCorsPolicy(IServiceCollection services)
        {
            services.AddCors(options =>
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

        private void ConfigureDatabaseServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(opt =>
                opt.UseSqlServer(Configuration["Database:OWarcade"]));

            services.AddHangfire(x => x.UseSqlServerStorage(Configuration["Database:Hangfire"]));
            services.AddHangfireServer();
        }
    }
}