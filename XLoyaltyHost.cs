using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

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

    private static void ConfigureUrls(IWebHostBuilder webBuilder, ushort port) // Manually specify the port
    {
        webBuilder.UseUrls($"http://localhost:{port}");
    }

    private static void ConfigureApplication(IWebHostBuilder webBuilder, string keysFile)
    {
        ConfigureAppConfiguration(webBuilder, keysFile);
        ConfigureAppEndpoints(webBuilder);
    }

    private static void ConfigureAppConfiguration(IWebHostBuilder webBuilder, string keysFile) // Configure the application secret keys file
    {
        webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            // Add external file
            config.AddJsonFile(keysFile, optional: false, reloadOnChange: true);
        });
    }

    private static void ConfigureAppEndpoints(IWebHostBuilder webBuilder) // Configure the application endpoints
    {
        webBuilder.Configure(app => // Configure application's HTTP request pipeline
        {
            app.UseRouting();  // Add routing
            app.UseEndpoints(endpoints => // Add endpoints
            {
                ConfigureVerifyUsernameEndpoint(endpoints);
                ConfigureGetFollowingFollowersLists(endpoints);
            });
        });
    }

    private static void MaximizeWebDriver(IWebDriver webDriver, WebDriverWait waiter, Random randomDelay) // Maximizes the web driver safely
    {
        waiter.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete")); // Wait for the driver to load
        Thread.Sleep((int)(randomDelay.NextSingle() * 2 + 1));
        webDriver.Manage().Window.Maximize(); // Maximize the browser for a better experience
        Console.WriteLine("WEB DRIVER MAXIMIZED!");
    }

    private static void LoadSignInAndWaitForAuthentication(IWebDriver webDriver, WebDriverWait waiter, string username) // Loads the sign in page and waits for the user to sign in
    {
        webDriver.Navigate().GoToUrl($"https://x.com/i/flow/login?redirect_after_login=%2F{username}"); // Have the user sign in
        waiter.Until(d => d.Url.Contains($"https://x.com/i/flow/login?redirect_after_login=%2F{username}")); // Wait for sign in page to load
        Console.WriteLine("USER IS TRYING TO SIGN IN!");
        waiter.Until(d => d.Url.Contains($"https://x.com/{username}?mx=2")); // Wait for the user to sign in
        Console.WriteLine("USER SIGNED IN!");
    }

    private static List<string> LoadPageScrollAndExtractUniqueUsernames(IWebDriver webDriver, string pageToLoad, WebDriverWait waiter, Random randomDelay, string username)
    {
        List<string> usernames = []; // Collect username followers
        long currentScrollHeight = 0;
        long previousScrollHeight = 0;
        webDriver.Navigate().GoToUrl(pageToLoad); // Navigate to the following tab
        waiter.Until(d =>
            d.Url.Contains(pageToLoad) &&
            d.FindElements(By.CssSelector(".css-1jxf684.r-bcqeeo.r-1ttztb7.r-qvutc0.r-poiln3")).Count > 1 &&
            ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete")
        );
        Console.WriteLine("NAVIGATED TO PAGE! " + pageToLoad);
        IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
        js.ExecuteScript("document.body.style.zoom='25%';");
       
        Thread.Sleep((int)(randomDelay.NextSingle() * 2 + 2));
        bool finalScrape = false;
        while (true) // Collect while scrolling
        {
            foreach (IWebElement element in webDriver.FindElements(By.CssSelector(".css-175oi2r.r-vacyoi.r-ttdzmv")))
            {
                js.ExecuteScript("arguments[0].parentNode.removeChild(arguments[0]);", element);
            }
            Thread.Sleep((int)(randomDelay.NextSingle() * 2) + 2);
            foreach (IWebElement element in webDriver.FindElements(By.CssSelector(".css-1jxf684.r-bcqeeo.r-1ttztb7.r-qvutc0.r-poiln3")))
            {
                string text;
                try
                {
                    string cssColorValue = element.GetCssValue("color");
                    if (cssColorValue == "rgb(29, 155, 240)") // Skip the usernames from bios
                    {
                        continue;
                    }
                    text = element.Text;
                    if (text.Contains("@") && text.StartsWith("@"))
                    {
                        usernames.Add(text.Substring(1));
                    }
                }
                catch (StaleElementReferenceException e)
                {
                    Console.WriteLine(e.StackTrace);
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    continue;
                }
            }
            webDriver.FindElement(By.TagName("body")).SendKeys(OpenQA.Selenium.Keys.PageDown); // Scroll down
            waiter.Until(d =>
                d.Url.Contains(pageToLoad) &&
                d.FindElements(By.CssSelector(".css-1jxf684.r-bcqeeo.r-1ttztb7.r-qvutc0.r-poiln3")).Count > 1 &&
                ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete")
            );
            currentScrollHeight = (long)((IJavaScriptExecutor)webDriver).ExecuteScript("return document.body.scrollHeight"); // Get the current scroll height
            if (currentScrollHeight == previousScrollHeight && !finalScrape) // Check if we've reached the bottom
            {
                finalScrape = true;
            }
            else if (finalScrape)
            {
                break;
            }
            previousScrollHeight = currentScrollHeight; // Set the new previous scroll height
        }
        return usernames.ToHashSet().ToList();
    }

    private static void ConfigureGetFollowingFollowersLists(IEndpointRouteBuilder endpoints)
    {
        _ = endpoints.MapGet("/FollowingFollowersLists", static async (string username, IConfiguration configuration, HttpResponse response) =>
        {
            Random randomDelay = new Random(); // Help mimic human delay
            List<string> followersToFollowBack = []; // will contain followers user is not following to follow
            List<string> nonFollowersToUnFollow = [];
            Dictionary<byte, List<string>> followersToFollowUnfollow = [];
            IWebDriver webDriver = new FirefoxDriver(); // Reject Google's tentacles
            webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromMilliseconds(1000000); // Prevent the web driver from timing out during extraction
            WebDriverWait waiter = new WebDriverWait(webDriver, TimeSpan.FromSeconds(30)); // Have the waiter timeout at 30 seconds for poopy connections
            try
            {
                MaximizeWebDriver(webDriver, waiter, randomDelay);
                LoadSignInAndWaitForAuthentication(webDriver, waiter, username);
                List<string> followers = LoadPageScrollAndExtractUniqueUsernames(webDriver, $"https://x.com/{username}/followers", waiter, randomDelay, username);
                List<string> followings = LoadPageScrollAndExtractUniqueUsernames(webDriver, $"https://x.com/{username}/following", waiter, randomDelay, username);
                followersToFollowBack = GetFollowersImNonFollowing(followers, followings); // will contain followers user is not following to follow
                nonFollowersToUnFollow = GetNonFollowersImFollowing(followers, followings); // will contain following not follower to unfollow
                followersToFollowUnfollow.Add(0, followersToFollowBack);
                followersToFollowUnfollow.Add(1, nonFollowersToUnFollow);
            }
            catch (HttpRequestException)
            {
                return Results.BadRequest(); // Something went horribly wrong on our end
            }
            finally
            {
                if (webDriver != null) // Close the web browser after it's done extracting the usernames
                {
                    webDriver.Close();
                    webDriver.Quit();
                    webDriver.Dispose();
                }
            }
            return Results.Ok(JsonSerializer.Serialize(followersToFollowUnfollow));
        }).WithName("FollowingFollowersLists");
    }

    private static List<string> GetFollowersImNonFollowing(List<string> followers, List<string> followings)
    {
        List<string> followersImNotFollowing = [];
        foreach (string follower in followers)
        {
            if (!followings.Contains(follower))
            {
                followersImNotFollowing.Add(follower);
            }
        }
        return followersImNotFollowing;
    }

    private static List<string> GetNonFollowersImFollowing(List<string> followers, List<string> followings)
    {
        List<string> nonFollowersImFollowing = [];
        foreach (string following in followings)
        {
            if (!followers.Contains(following))
            {
                nonFollowersImFollowing.Add(following);
            }
        }
        return nonFollowersImFollowing;
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