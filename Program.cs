using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

// Create and configure the host builder
var builder = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
{
    // Explicitly bind to port 5115
    webBuilder.UseUrls("http://localhost:5115");
    // Configure application configuration
    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
    {
        // Add external file
        config.AddJsonFile("secrets.json", optional: false, reloadOnChange: true);

    });
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
});
// Setup method that tests endpoint to determine if a user exists
static async Task<XLoyaltyResponseCode> VerifyUsername(string xUsername)
{
    // Create local http client
    var client = new HttpClient();
    // Get a request from a response sent and Dispose client
    var response = await client.GetAsync($"http://localhost:5115/VerifyUsername?username={xUsername}");
    client.Dispose();
    // Respond based on the response
    if (response.IsSuccessStatusCode)
    {
        string responseString = await response.Content.ReadAsStringAsync();
        XLoyaltyResponseCode responseCode = (XLoyaltyResponseCode)sbyte.Parse(responseString);
        return responseCode;
    }
    // Bad request
    else
    {
        return XLoyaltyResponseCode.Error;
    }
}
// Build the custom host builder
var host = builder.Build();
// Start the host in the background
var hostTask = host.RunAsync();




// Custom intro label text
string introLabelText = "\nWelcome to X LOYALTY!\nEnter an X account username (alphanumeric)\n to determine if it exists or not!";
// Create the form builder to build the x loyalty form
XLoyaltyFormBuilder form = new XLoyaltyFormBuilder("X LOYALTY", new Size(420, 420))
.AddIntroLabel(introLabelText, new Point(0, 0), new Size(420, 60))
.AddUsernameTextBox("Enter Username Here!", new Point(105, 75), new Size(210, 180))
.AddElonSmilingPictureBox("elonSmiling.jpg", new Point(0, 180), new Size(420, 200))
.AddElonFrowningPictureBox("elonFrowning.jpeg", new Point(0, 180), new Size(420, 200))
.AddElonGoFYourselfPictureBox("elonFYourself.jpg", new Point(0, 180), new Size(420, 200))
.AddSubmitTryAgainButton("Go", new Point(180, 120), new Size(60, 30), VerifyUsername, host)
.Run();