using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public class XLoyaltyHost
{
    private IHostBuilder hostBuilder;
    public XLoyaltyHost(string[] args, ushort port)
    {
        hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            ConfigureUrls(webBuilder, port);
            ConfigureApplication(webBuilder);
        });
    }

    private static void ConfigureUrls(IWebHostBuilder webBuilder, ushort port)
    {
        webBuilder.UseUrls($"http://localhost:{port}");
    }

}