using Amazon.Lambda.AspNetCoreServer;
using System.Diagnostics.CodeAnalysis;

namespace BookLendingApp;

[ExcludeFromCodeCoverage]
public class LambdaEntryPoint : APIGatewayProxyFunction
{
    protected override void Init(IWebHostBuilder builder)
    {
        builder.UseContentRoot(Directory.GetCurrentDirectory())
               .UseStartup<Startup>();
    }
}