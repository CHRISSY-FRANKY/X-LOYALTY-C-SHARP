using System.Security.Authentication;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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
                ConfigureVerifyUsernameEndpoint(endpoints);
                ConfigureGetFollowingFollowersLists(endpoints);
            });
        });
    }

    private static void ConfigureGetFollowingFollowersLists(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/FollowingFollowersLists", async (string username, IConfiguration configuration, HttpResponse response) =>
        {
            Thread.Sleep(3000);
            try
            {
                IWebDriver webDriver = new ChromeDriver(); // Google's tentacles are everywhere
                Random randomDelay = new Random(); // Help mimic human delay

                webDriver.Manage().Window.Maximize(); // Maximize the browser for a better experience
                webDriver.Navigate().GoToUrl($"https://x.com/i/flow/login?redirect_after_login=%2F{username}"); // Have the user sign in
                byte signInStage = 0; // Keep track of the user logging in
                try
                {

                    while (true)
                    {

                        if (webDriver.Url == $"https://x.com/i/flow/login?redirect_after_login=%2F{username}" && signInStage == 0)
                        {
                            //Console.Write("USER IS TRYING TO SIGN IN!");
                            signInStage = 1;
                        }
                        else if (webDriver.Url == $"https://x.com/{username}" && signInStage == 1)
                        {
                            //Console.Write("USER SIGNED IN!");
                            signInStage = 2;
                        }
                        else if (signInStage == 2)
                        {
                            break;
                        }
                        Thread.Sleep(1000); // check every 1 second
                    }
                    // Navigate to the followers website
                    webDriver.Navigate().GoToUrl($"https://x.com/{username}/followers");
                    int userDelay = randomDelay.Next(2000, 3000); // Allow the page to load / mimic delay
                    Thread.Sleep(userDelay);
                    Console.WriteLine("Followers:"); // Print the usernames of each follower
                    foreach (IWebElement element in webDriver.FindElements(By.CssSelector(".css-1jxf684.r-bcqeeo.r-1ttztb7.r-qvutc0.r-poiln3")))
                    {
                        string text = element.Text;
                        if (text.Contains("@") && text.StartsWith("@"))
                        {
                            Console.WriteLine(text);
                        }
                    }
                    webDriver.Navigate().GoToUrl($"https://x.com/{username}/following"); // Click on the "Following" tab
                    userDelay = randomDelay.Next(2000, 3000); // Allow the page to load / mimic delay
                    Thread.Sleep(userDelay);
                    Console.WriteLine("Following:"); // Print the usernames of each following user
                    foreach (IWebElement element in webDriver.FindElements(By.CssSelector(".css-1jxf684.r-bcqeeo.r-1ttztb7.r-qvutc0.r-poiln3")))
                    {
                        string text = element.Text;
                        if (text.Contains("@") && text.StartsWith("@"))
                        {
                            Console.WriteLine(text);
                        }
                    }
                }
                finally
                {
                    if (webDriver != null)
                    {
                        webDriver.Quit();
                        webDriver.Dispose();
                    }
                }
                return Results.Ok("test test test");
            }
            catch (HttpRequestException)
            {
                return Results.BadRequest(); // Something went horribly wrong on our end
            }
        }).WithName("FollowingFollowersLists");
    }

    private static void ConfigureVerifyUsernameEndpoint(IEndpointRouteBuilder endpoints)
    {
        // Verify Username Endpoint
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