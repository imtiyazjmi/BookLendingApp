using Amazon.Lambda.AspNetCoreServer;

namespace BookLendingApp;

public class LambdaEntryPoint : APIGatewayProxyFunction
{
    protected override void Init(IWebHostBuilder builder)
    {
        builder.UseContentRoot(Directory.GetCurrentDirectory())
               .UseStartup<Startup>();
    }
}