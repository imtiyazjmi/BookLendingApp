using Microsoft.EntityFrameworkCore;
using BookLendingApp.Data;
using BookLendingApp.Services;
using BookLendingApp.Repositories;
using BookLendingApp.Interfaces;
using Amazon.SimpleSystemsManagement;
using Amazon.Extensions.NETCore.Setup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure database
var useSSM = builder.Configuration.GetValue<bool>("UseSSM");
if (useSSM)
{
    builder.Services.AddAWSService<IAmazonSimpleSystemsManagement>();
    builder.Services.AddSingleton<ParameterStoreService>();
}

builder.Services.AddSingleton<DatabaseConfigurationService>();

builder.Services.AddDbContext<BookContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var useSSM = configuration.GetValue<bool>("UseSSM");
    
    string connectionString;
    if (useSSM)
    {
        // For Lambda, use environment variables when SSM is enabled
        var host = Environment.GetEnvironmentVariable("POSTGRESQL_HOST") ?? "localhost";
        var database = Environment.GetEnvironmentVariable("POSTGRESQL_DATABASE") ?? "booklendingdb";
        var username = Environment.GetEnvironmentVariable("POSTGRESQL_USERNAME") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("POSTGRESQL_PASSWORD") ?? "admin";
        connectionString = $"Host={host};Database={database};Username={username};Password={password}";
    }
    else
    {
        var pgSettings = configuration.GetSection("PostgreSQL").Get<PostgreSqlSettings>();
        connectionString = $"Host={pgSettings.Host};Database={pgSettings.Database};Username={pgSettings.Username};Password={pgSettings.Password}";
    }
    
    options.UseNpgsql(connectionString);
});

// Register services
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

app.Run();
