using Microsoft.EntityFrameworkCore;
using BookLendingApp.Data;
using BookLendingApp.Services;
using BookLendingApp.Repositories;
using BookLendingApp.Interfaces;
using Amazon.SimpleSystemsManagement;
using Amazon.Extensions.NETCore.Setup;

namespace BookLendingApp;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        var useSSM = Configuration.GetValue<bool>("UseSSM");
        if (useSSM)
        {
            services.AddAWSService<IAmazonSimpleSystemsManagement>();
            services.AddSingleton<ParameterStoreService>();
        }

        services.AddSingleton<DatabaseConfigurationService>();

        services.AddDbContext<BookContext>((serviceProvider, options) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            logger.LogInformation("configuration: {configuration}", configuration);
            var useSSM = configuration.GetValue<bool>("UseSSM");
            logger.LogInformation("useSSM: {UseSSM}", useSSM);
            
            string connectionString;
            
            // Check if environment variables are available (Lambda deployment)
            var envHost = Environment.GetEnvironmentVariable("POSTGRESQL_HOST");
            if (!string.IsNullOrEmpty(envHost))
            {
                var host = envHost;
                var database = Environment.GetEnvironmentVariable("POSTGRESQL_DATABASE") ?? "postgres";
                var username = Environment.GetEnvironmentVariable("POSTGRESQL_USERNAME") ?? "masteruser";
                var password = Environment.GetEnvironmentVariable("POSTGRESQL_PASSWORD") ?? "Password2123";
                connectionString = $"Host={host};Database={database};Username={username};Password={password}";
                logger.LogInformation("Using environment variables for database connection: Host={Host}, Database={Database}, Username={Username}", host, database, username);
            }
            else
            {
                var pgSettings = configuration.GetSection("PostgreSQL").Get<PostgreSqlSettings>();
                logger.LogInformation("pgSettings: {PgSettings}", pgSettings?.ToString() ?? "null");
                
                if (pgSettings == null)
                {
                    logger.LogWarning("PostgreSQL configuration not found, using default connection");
                    connectionString = "Host=localhost;Database=booklendingdb;Username=postgres;Password=admin";
                }
                else
                {
                    logger.LogInformation("Using PostgreSQL configuration from appsettings");
                    connectionString = $"Host={pgSettings.Host};Database={pgSettings.Database};Username={pgSettings.Username};Password={pgSettings.Password}";
                }
            }
            
            logger.LogInformation("Database connection configured successfully" + connectionString);
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookService, BookService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core Minimal API on AWS Lambda");
            });
        });
    }
}
