using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public class XLoyaltyHost
{
    private IHostBuilder hostBuilder;
    public XLoyaltyHost(string[] args)
    {
        hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            ConfigureUrls(webBuilder);
            ConfigureApplication(webBuilder);
        });
    }
}