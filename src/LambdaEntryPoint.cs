using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace BookLendingApp;

public class LambdaEntryPoint : APIGatewayProxyFunction
{
    protected override void Init(IWebHostBuilder builder)
    {
        builder.UseContentRoot(Directory.GetCurrentDirectory())
               .UseStartup<Startup>();
    }

    protected override void PostCreateHost(IWebHost webHost)
    {
        base.PostCreateHost(webHost);
    }
}