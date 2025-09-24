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

builder.Services.AddDbContext<BookContext>(async (serviceProvider, options) =>
{
    var configService = serviceProvider.GetRequiredService<DatabaseConfigurationService>();
    var connectionString = await configService.GetConnectionStringAsync();
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
