using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public class XLoyaltyHost
{
    private IHostBuilder hostBuilder;
    private IHost builtHost;
    public XLoyaltyHost(string[] args, ushort port, string keysFile)
    {
        hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            ConfigureUrls(webBuilder, port);
            ConfigureApplication(webBuilder, keysFile);
        });
    }

    // Manually specify the port
    private static void ConfigureUrls(IWebHostBuilder webBuilder, ushort port)
    {
        webBuilder.UseUrls($"http://localhost:{port}");
    }

    private static void ConfigureApplication(IWebHostBuilder webBuilder, string keysFile)
    {
        ConfigureAppConfiguration(webBuilder, keysFile);
        ConfigureAppEndpoints(webBuilder);
    }

    // Configure the application secret keys file
    private static void ConfigureAppConfiguration(IWebHostBuilder webBuilder, string keysFile)
    {
        webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            // Add external file
            config.AddJsonFile(keysFile, optional: false, reloadOnChange: true);
        });
    }

    // Configure the application endpoints
    private static void ConfigureAppEndpoints(IWebHostBuilder webBuilder)
    {
        // Configure application's HTTP request pipeline
        webBuilder.Configure(app =>
        {
            // Add routing
            app.UseRouting();
            // Add endpoints
            app.UseEndpoints(endpoints =>
            {
                // Define Get Lists of Followers and Following
                endpoints.MapGet("/VerifyUsername", async (string username, IConfiguration configuration, HttpResponse response) =>
                {
                    try
                    {
                        // Setup http client
                        using var httpClient = new HttpClient();
                        // Save the bearer token
                        string bearerToken = configuration["BearerToken"];
                        // Add the authorization header
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
                        // Build the url to request the existence of the username provided
                        string url = $"https://api.twitter.com/2/users/by/username/{username}";
                        //Console.WriteLine(bearerToken); Checking configuration settings
                        //Console.WriteLine(url); Checking ur
                        // Make the custom request
                        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);
                        // Get the response body
                        string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                        // Parse the response body into a json
                        var jsonDocument = JsonDocument.Parse(responseBody);
                        JsonElement rootJsonElement = jsonDocument.RootElement;
                        // Too many requests
                        if (rootJsonElement.TryGetProperty("title", out var property))
                        {
                            //Console.WriteLine(property.ToString());
                            return Results.Ok(XLoyaltyResponseCode.RequestsLimited);

                        }
                        // Username exists
                        else if (rootJsonElement.TryGetProperty("data", out var prop))
                        {
                            return Results.Ok(XLoyaltyResponseCode.UsernameExists);
                        }
                        // Username doesn't exist
                        else
                        {

                            return Results.Ok(XLoyaltyResponseCode.UsernameNonExistant);
                        }
                    }
                    catch (HttpRequestException)
                    {
                        // Something went horribly wrong on our end
                        return Results.BadRequest();
                    }
                }).WithName("VerifyUsername");
            });
        });
    }

    public XLoyaltyHost BuildHost()
    {
        builtHost = hostBuilder.Build();
        return this;
    }

    public XLoyaltyHost RunHost()
    {
        builtHost.RunAsync();
        return this;

    }

    public IHost GetHost()
    {
        return this.builtHost;
    }
}