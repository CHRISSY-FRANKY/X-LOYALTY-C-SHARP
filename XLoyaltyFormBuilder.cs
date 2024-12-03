// X Loyalty Form Builder Class to improve readability
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class XLoyaltyFormBuilder
{
    private Form form;
    private Label introLabel;
    private TextBox usernameTextBox;
    private PictureBox elonFrowningPictureBox;
    private PictureBox elonSmilingPictureBox;
    private PictureBox elonGoFYourselfPictureBox;
    private Button submitTryAgainButton;
    private Panel usernamesScrollablePanel;

    public XLoyaltyFormBuilder(Size formSize)
    {
        // Set application settings
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        // Create form based on arguments provided
        form = new Form
        {
            Text = "X LOYALTY",
            MaximizeBox = false,
            MinimizeBox = false,
            FormBorderStyle = FormBorderStyle.FixedSingle,
            BackColor = Color.Black,
            Size = formSize
        };
    }

    // Adds Intro Label to Form
    public XLoyaltyFormBuilder AddIntroLabel(Point location, Size size)
    {
        // Create the label
        introLabel = new()
        {
            Text = "\nWelcome to X LOYALTY!\nEnter an X account username (alphanumeric)\nto determine if it exists or not!",
            Location = location,
            Size = size,
            BackColor = Color.Black,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.TopCenter
        };
        form.Controls.Add(introLabel);
        return this;
    }

    // Adds Username to TextBox
    public XLoyaltyFormBuilder AddUsernameTextBox(Point location, Size size)
    {
        // Add the text box to the form
        usernameTextBox = new()
        {
            Text = "Enter Username Here!",
            Location = location,
            Size = size,
            BackColor = Color.White,
            ForeColor = Color.Black,
            TextAlign = HorizontalAlignment.Center
        };
        // Add the textbox to the form
        form.Controls.Add(usernameTextBox);
        return this;
    }

    // Add Elon Smiling Picture Box
    public XLoyaltyFormBuilder AddElonSmilingPictureBox(string fileName, Point location, Size size)
    {
        // Create elon smiling to show the user when the username exists
        elonSmilingPictureBox = new PictureBox
        {
            Image = Image.FromFile(fileName),
            Location = location,
            Size = size,
            SizeMode = PictureBoxSizeMode.Zoom,
        };
        form.Controls.Add(elonSmilingPictureBox);
        elonSmilingPictureBox.Visible = false;
        // When the username text box is clicked, the picture box is hidden
        usernameTextBox.Click += (sender, e) =>
        {
            elonSmilingPictureBox.Visible = false;
        };
        return this;
    }

    // Add Elon Frowning Picture Box
    public XLoyaltyFormBuilder AddElonFrowningPictureBox(string fileName, Point location, Size size)
    {
        // Create elon frowning to show the user when the username does not exist
        elonFrowningPictureBox = new PictureBox
        {
            Image = Image.FromFile(fileName),
            Location = location,
            Size = size,
            SizeMode = PictureBoxSizeMode.Zoom,
        };
        form.Controls.Add(elonFrowningPictureBox);
        elonFrowningPictureBox.Visible = false;
        // When the username text box is selected, hide picture box
        usernameTextBox.Click += (sender, e) =>
        {
            elonFrowningPictureBox.Visible = false;
        };
        return this;
    }

    // Add Elon Go F Yourself Picture Box
    public XLoyaltyFormBuilder AddElonGoFYourselfPictureBox(string fileName, Point location, Size size)
    {
        // Create elon f yourself to show the user you are out of requests
        elonGoFYourselfPictureBox = new PictureBox
        {
            Image = Image.FromFile(fileName),
            Location = location,
            Size = size,
            SizeMode = PictureBoxSizeMode.Zoom,
        };
        form.Controls.Add(elonGoFYourselfPictureBox);
        elonGoFYourselfPictureBox.Visible = false;
        // When the username text box is selected, hide the picture box
        usernameTextBox.Click += (sender, e) =>
        {
            // Just hide the elon images
            elonGoFYourselfPictureBox.Visible = false;
        };
        return this;
    }

    public XLoyaltyFormBuilder AddFollowersFollowingScrollControl()
    {
        // Create a new ScrollablePanel
        usernamesScrollablePanel = new Panel
        {
            Location = new Point(0, 180),
            Size = new Size(420, 200),
            AutoScroll = true,
        };
        // Add the ScrollablePanel to the Form
        form.Controls.Add(usernamesScrollablePanel);
        usernamesScrollablePanel.Visible = false;
        return this;
    }

    // Returns whether or not to proceed to the next action after the username has been verified
    private void UpdateElonMuskVisibility(XLoyaltyResponseCode response)
    {
        switch (response)
        {
            // Show elon smiling
            case XLoyaltyResponseCode.UsernameExists:
                elonFrowningPictureBox.Visible = false;
                elonSmilingPictureBox.Visible = true;
                elonGoFYourselfPictureBox.Visible = false;
                break;
            // Elon frowning
            case XLoyaltyResponseCode.UsernameNonExistant:
                elonFrowningPictureBox.Visible = true;
                elonSmilingPictureBox.Visible = false;
                elonGoFYourselfPictureBox.Visible = false;
                break;
            // Elon telling you to go duck yourself
            case XLoyaltyResponseCode.RequestsLimited:
                elonFrowningPictureBox.Visible = false;
                elonSmilingPictureBox.Visible = false;
                elonGoFYourselfPictureBox.Visible = true;
                break;
            // Something went horribly wrong through the pipeline
            case XLoyaltyResponseCode.Error:
                introLabel.Text = "Something went wrong on our end!";
                Thread.Sleep(5000);
                Application.Exit();
                break;
        }
    }

    private async Task<string> GetFollowingFollowersLists(string xUsername)
    {
        // Create local http client
        using (var client = new HttpClient())
        {
            // Get a response from the request sent
            var response = await client.GetAsync($"http://localhost:5115/FollowingFollowersLists?username={xUsername}");
            string responseString = "";

            // Response based on the response
            if (response.IsSuccessStatusCode)
            {
                responseString = await response.Content.ReadAsStringAsync(); // Get the response string
                return responseString;
            }
            else
            {
                return responseString;
            }
        }

    }

    // Setup method that tests endpoint to determine if a user exists
    private static async Task<XLoyaltyResponseCode> VerifyUsername(string xUsername)
    {
        // Create local http client
        using (var client = new HttpClient()) // Using HttpClientFactory or IHttpClientFactory would be more appropriate for production
        {
            // Get a response from a request sent
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

    // Add a button to submit username and to try again
    public XLoyaltyFormBuilder AddSubmitTryAgainButton(Point location, Size size, IHost? host)
    {
        this.form.FormClosing += (sender, e) =>
        { // Dispose the host before closing the form
            host.Dispose();
        };
        // Create a submit try again button to submit a request to the x api for username search
        submitTryAgainButton = new Button
        {
            Text = "Go",
            Location = location,
            Size = size,
            BackColor = Color.White,
            ForeColor = Color.Black,
            TextAlign = ContentAlignment.MiddleCenter
        };
        form.Controls.Add(submitTryAgainButton);
        // Add an asynchronous event handler
        submitTryAgainButton.Click += async (sender, e) =>
        {
            // Disable the button right away to not lodge the pipeline
            submitTryAgainButton.Enabled = false;
            // Just hide the elon images
            elonFrowningPictureBox.Visible = false;
            elonSmilingPictureBox.Visible = false;
            elonGoFYourselfPictureBox.Visible = false;
            // X usernames are only supposed to be alphanumeric
            if (usernameTextBox.Text.All(char.IsLetterOrDigit))
            {
                // Obtain response code
                XLoyaltyResponseCode response = await VerifyUsername(usernameTextBox.Text);
                UpdateElonMuskVisibility(response);
                // Username exists, meaning we can access a following and followers list
                if (response == XLoyaltyResponseCode.UsernameExists)
                {
                    // Disable form
                    form.Enabled = false;
                    // Username exists, get lists of following and followers
                    string responseString = await GetFollowingFollowersLists(usernameTextBox.Text);
                    AbstractFollowingFollowersListsToBuildFormLinks(responseString);
                    usernamesScrollablePanel.Visible = true;
                    // Enable form
                    form.Enabled = true;
                }
            }
            else
            {
                submitTryAgainButton.Text = "Try Again";
            }
            submitTryAgainButton.Enabled = true;
        };
        // Kill the host when you kill the form
        form.FormClosed += async (sender, e) => await host.StopAsync();
        return this;
    }

    private int BuildFormLinksOfUsernamesYoureNotFollowingBack(List<string> usernameList)
    {
        int yLocation = 20;
        int xLocation = 5;
        int count = usernameList.Count();
        int index = count - usernameList.Count();
        while (index < count)
        {
            // Create a new Button
            Button button = new Button();
            button.Text = usernameList[index];
            button.Location = new Point(xLocation, yLocation);
            // Add a click event handler to the button
            button.Click += (sender, args) =>
            {
                // Open Google in the default web browser
                Process.Start(new ProcessStartInfo($"https://x.com/{button.Text}") { UseShellExecute = true });
            };
            // Add the button to the Panel
            usernamesScrollablePanel.Controls.Add(button);
            xLocation += 5;
            if ((index + 1) % 3 == 0)
            {
                xLocation = 0;
                yLocation += 20;
            }
            index += 1;
        }
        return yLocation;
    }

    private void BuildFormLinksOfUsernamesNotFollowingYouBack(int yLocation, List<string> usernameList)
    {
        yLocation += 40;
        int xLocation = 5;
        int count = usernameList.Count();
        int index = count - usernameList.Count();
        while (index < count)
        {
            // Create a new Button
            Button button = new Button();
            button.Text = usernameList[index];
            button.Location = new Point(xLocation, yLocation);
            // Add a click event handler to the button
            button.Click += (sender, args) =>
            {
                // Open Google in the default web browser
                Process.Start(new ProcessStartInfo($"https://x.com/{button.Text}") { UseShellExecute = true });
            };
            // Add the button to the Panel
            usernamesScrollablePanel.Controls.Add(button);
            xLocation += 5;
            if ((index + 1) % 3 == 0)
            {
                xLocation = 0;
                yLocation += 20;
            }
            index += 1;
        }
    }

    private void AbstractFollowingFollowersListsToBuildFormLinks(string responseString)
    {
        // Get rid of the dictionary symbols
        responseString = responseString.Substring(1, responseString.Length - 2);
        responseString = responseString.Replace("\"", "");
        // Keep track of the list and username
        bool isTraversingList = false;
        bool isTraversingUsername = false;
        bool isFirstList = true;
        // Save the usernames in each of their respective lists
        List<string> followersToFollowBack = [];
        List<string> nonFollowersToUnFollow = [];
        StringBuilder currentUsername = new StringBuilder();
        for (int i = 0; i < responseString.Length; i++)
        {
            char currentChar = responseString.ElementAt(i);
            // Searches for list
            if (!isTraversingList && currentChar == '[')
            {
                // List found
                isTraversingList = true;
            }
            else if (isTraversingList && currentChar == ']')
            {
                isTraversingList = false;
                isFirstList = false;
            }
            // Traversing List, Searches for username
            else if (isTraversingList)
            {

                // Username found
                if (!isTraversingUsername && Char.IsAsciiLetter(currentChar))
                {
                    isTraversingUsername = true;
                }
                // Username has ended
                else if (isTraversingUsername && currentChar == ',')
                {
                    isTraversingUsername = false;
                    if (isFirstList)
                    {
                        followersToFollowBack.Add(currentUsername.ToString());
                    }
                    else
                    {
                        nonFollowersToUnFollow.Add(currentUsername.ToString());
                    }
                    currentUsername = currentUsername.Clear();
                }
                if (isTraversingUsername)
                {
                    currentUsername.Append(currentChar);
                }

            }
        }
        int yPosition = BuildFormLinksOfUsernamesYoureNotFollowingBack(followersToFollowBack);
        BuildFormLinksOfUsernamesNotFollowingYouBack(yPosition, nonFollowersToUnFollow);
    }


    public XLoyaltyFormBuilder Run()
    {
        Application.Run(form);
        return this;
    }

    public Form GetForm()
    {
        return this.form;
    }
}
