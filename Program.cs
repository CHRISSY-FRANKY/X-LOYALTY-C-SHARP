public class Program
{
    public static void Main(string[] args)
    {
        // Create XLoyaltyHost
        XLoyaltyHost xLoyaltyHost = new XLoyaltyHost(args, 5115, "secrets.json").BuildHost().RunHost();

        // Create the form builder to build the x loyalty form
        XLoyaltyFormBuilder form = new XLoyaltyFormBuilder(new Size(420, 420))
            .AddIntroLabel(new Point(0, 0), new Size(420, 60))
            .AddUsernameTextBox(new Point(105, 75), new Size(210, 180))
            .AddElonSmilingPictureBox("elonSmiling.jpg", new Point(0, 180), new Size(420, 200))
            .AddElonFrowningPictureBox("elonFrowning.jpeg", new Point(0, 180), new Size(420, 200))
            .AddElonGoFYourselfPictureBox("elonFYourself.jpg", new Point(0, 180), new Size(420, 200))
            .AddSubmitTryAgainButton(new Point(180, 120), new Size(60, 30), VerifyUsername, xLoyaltyHost.GetHost())
            .Run();
    }

    // Setup method that tests endpoint to determine if a user exists
    public static async Task<XLoyaltyResponseCode> VerifyUsername(string xUsername)
    {
        // Create local http client
        using (var client = new HttpClient()) // Using HttpClientFactory or IHttpClientFactory would be more appropriate for production
        {
            // Get a request from a response sent
            var response = await client.GetAsync($"http://localhost:5115/VerifyUsername?username={xUsername}");

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
    }
}