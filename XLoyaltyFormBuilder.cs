using Microsoft.Extensions.Hosting;
using System.Diagnostics;

public class XLoyaltyFormBuilder
{
    private Form form;
    private Label introLabel;
    private TextBox usernameTextBox;
    private PictureBox elonFrowningPictureBox;
    private PictureBox elonSmilingPictureBox;
    private PictureBox elonGoFUrselfPictureBox;
    private Button submitTryAgainButton;
    private Panel usernamesScrollablePanel;

    public XLoyaltyFormBuilder(Size formSize) // Set application settings
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        form = new WinFormsFormBuilder().SetText("X LOYALTY").SetMaximizeBox(false).SetMinimizeBox(false).setFormBorderStyle(FormBorderStyle.FixedSingle).setBackColor(Color.Black).setSize(formSize);
    }

    public XLoyaltyFormBuilder AddIntroLabel(Point location, Size size) // Creates and adds intro label to Form
    {
        string text = "\nWelcome to X LOYALTY!\nEnter an X account username (alphanumeric)\nto determine if it exists or not!";
        introLabel = new WinFormsLabelBuilder().SetText(text).SetLocation(location).SetSize(size).SetBackColor(Color.Black).SetForeColor(Color.White).SetTextAlign(ContentAlignment.TopCenter);
        form.Controls.Add(introLabel);
        return this;
    }

    public XLoyaltyFormBuilder AddUsernameTextBox(Point location, Size size) // Creates and adds username text box to form
    {
        string text = "Enter Username Here!";
        usernameTextBox = new WinFormsTextBoxBuilder().SetText(text).SetLocation(location).SetSize(size).SetBackColor(Color.White).SetForeColor(Color.Black).SetTextAlign(HorizontalAlignment.Center);
        form.Controls.Add(usernameTextBox);
        return this;
    }

    public XLoyaltyFormBuilder AddElonSmilingPictureBox(string fileName, Point location, Size size) // Create and add Elon Smiling Picture Box
    {
        elonSmilingPictureBox = new WinFormsPictureBoxBuilder().SetImage(Image.FromFile(fileName)).SetLocation(location).SetSize(size).SetSizeMode(PictureBoxSizeMode.Zoom).SetVisible(false);
        form.Controls.Add(elonSmilingPictureBox);
        usernameTextBox.Click += (sender, e) => elonSmilingPictureBox.Visible = false; // Picture box is hidden when username text box is re activated
        return this;
    }

    public XLoyaltyFormBuilder AddElonFrowningPictureBox(string fileName, Point location, Size size) // Create and add elon frowning picture box
    {
        elonFrowningPictureBox = new WinFormsPictureBoxBuilder().SetImage(Image.FromFile(fileName)).SetLocation(location).SetSize(size).SetSizeMode(PictureBoxSizeMode.Zoom).SetVisible(false);
        form.Controls.Add(elonFrowningPictureBox);
        usernameTextBox.Click += (sender, e) => elonFrowningPictureBox.Visible = false; // Picture box is hidden when username text box is re activated
        return this;
    }

    public XLoyaltyFormBuilder AddElonGoFYourselfPictureBox(string fileName, Point location, Size size) // Create and add Elon Go F yourself picture box
    {
        elonGoFUrselfPictureBox = new WinFormsPictureBoxBuilder().SetImage(Image.FromFile(fileName)).SetLocation(location).SetSize(size).SetSizeMode(PictureBoxSizeMode.Zoom).SetVisible(false);
        form.Controls.Add(elonGoFUrselfPictureBox);
        usernameTextBox.Click += (sender, e) => elonGoFUrselfPictureBox.Visible = false; // Picture box is hidden when username text box is re activated
        return this;
    }

    public XLoyaltyFormBuilder AddUsernamesScrollPanel() // Create and add usernames scrollable panel
    {
        usernamesScrollablePanel = new WinFormsPanelBuilder().SetLocation(new Point(0, 175)).SetSize(new Size(420, 200)).SetAutoScroll(true);
        form.Controls.Add(usernamesScrollablePanel);
        return this;
    }

    private void UpdateElonMuskImages(bool frowning, bool smiling, bool goFYourself)
    {
        elonFrowningPictureBox.Visible = frowning;
        elonSmilingPictureBox.Visible = smiling;
        elonGoFUrselfPictureBox.Visible = goFYourself;
    }

    private void UpdateElonMuskImagesBasedOnResponse(XLoyaltyResponseCode response)
    {
        switch (response)
        {
            case XLoyaltyResponseCode.UsernameExists:
                UpdateElonMuskImages(false, true, false); // Only Show Elon Smiling Image
                break;
            case XLoyaltyResponseCode.UsernameNonExistant:
                UpdateElonMuskImages(true, false, false); // Only Show Elon Frowning Image
                break;
            case XLoyaltyResponseCode.RequestsLimited:
                UpdateElonMuskImages(false, false, true); // Only Show Elon Go F Yourself Image
                break;
            case XLoyaltyResponseCode.Error: // Something went horribly wrong through the pipeline
                introLabel.Text = "Something went wrong on our end!";
                Thread.Sleep(3000);
                Application.Exit();
                break;
        }
    }

    private async Task<string> GetFollowingFollowersLists(string xUsername)
    {
        using (var client = new HttpClient()) // Create local http client
        {
            var response = await client.GetAsync($"http://localhost:5115/FollowingFollowersLists?username={xUsername}"); // Get a response from the request sent
            string responseString = "";
            if (response.IsSuccessStatusCode) // Response based on the response
            {
                responseString = await response.Content.ReadAsStringAsync(); // Get the response string
                return responseString;
            }
            return responseString;
        }
    }

    private static async Task<XLoyaltyResponseCode> VerifyUsername(string xUsername) // Setup method that implments endpoint to determine if a user exists
    {
        using var client = new HttpClient(); // Using HttpClientFactory or IHttpClientFactory would be more appropriate for production
        HttpResponseMessage response = await client.GetAsync($"http://localhost:5115/VerifyUsername?username={xUsername}"); // Get a response from a request sent
        if (response.IsSuccessStatusCode) // Respond based on the response
        {
            string responseString = await response.Content.ReadAsStringAsync();
            XLoyaltyResponseCode responseCode = (XLoyaltyResponseCode)sbyte.Parse(responseString);
            return responseCode;
        }
        return XLoyaltyResponseCode.Error; // Bad request
    }

    private string[] GetListOfUsernamesYoureNotFollowingBack(string usernamesString)
    {
        int index = usernamesString.IndexOf('[');
        return usernamesString.Substring(index + 1).Replace('\"', ' ').Replace('\\', ' ').Split(",");
    }

    private string[] GetListOfUsernamesNotFollowingYouBack(string usernamesString)
    {
        int index = usernamesString.IndexOf('[');
        return usernamesString.Substring(index + 2, usernamesString.Length - 12).Replace('\"', ' ').Replace('\\', ' ').Split(",");
    }

    public XLoyaltyFormBuilder AddSubmitTryAgainButton(Point location, Size size, IHost? host) // Add button to submit username as a request to x api or to try again
    {
        form.FormClosing += (sender, e) => host.Dispose(); // Dispose the host before closing the form
        submitTryAgainButton = new WinFormsButtonBuilder().SetText("GO").SetLocation(location).SetSize(size).SetBackColor(Color.White).SetForeColor(Color.Black).SetTextAlign(ContentAlignment.BottomCenter);
        form.Controls.Add(submitTryAgainButton);
        submitTryAgainButton.Click += async (sender, e) => // Add an asynchronous event handler
        {
            submitTryAgainButton.Enabled = false;  // Disable the button right away to not lodge the pipeline
            UpdateElonMuskImages(false, false, false);
            if (usernameTextBox.Text.All(c => char.IsLetterOrDigit(c) || c == '_')) // X usernames are only supposed to be alphanumeric or have an underscore
            {
                XLoyaltyResponseCode response = await VerifyUsername(usernameTextBox.Text); // Obtain response code
                UpdateElonMuskImagesBasedOnResponse(response);
                if (response == XLoyaltyResponseCode.UsernameExists) // Username exists, meaning we can access a following and followers list
                {
                    form.Enabled = false; // Disable form
                    string responseString = await GetFollowingFollowersLists(usernameTextBox.Text); // Username exists, get lists of following and followers
                    Console.WriteLine(responseString);
                    string[] usernamesStrings = responseString.Split("],"); // Separate the response string into two lists
                    string[] usernamesYoureNotFollowingBack = GetListOfUsernamesYoureNotFollowingBack(usernamesStrings[0]);
                    string[] usernamesNotFollowingYouBack = GetListOfUsernamesNotFollowingYouBack(usernamesStrings[1]);
                    int yPosition = BuildFormLinkButtonsOfUsernames("Who You're Not Following Back!", 0, usernamesYoureNotFollowingBack);
                    BuildFormLinkButtonsOfUsernames("Who Is Not Following You Back!", yPosition + 45, usernamesNotFollowingYouBack);
                    UpdateElonMuskImages(false, false, false);
                    usernamesScrollablePanel.Visible = true;
                    form.Enabled = true; // Enable form
                    form.BringToFront();
                }
            }
            else
            {
                submitTryAgainButton.Text = "Try Again";
            }
            submitTryAgainButton.Enabled = true;
        };
        form.FormClosed += async (sender, e) => await host.StopAsync(); // Kill the host when you kill the form
        return this;
    }

    private int BuildFormLinkButtonsOfUsernames(string title, int yLocation, string[] usernameList)
    {
        Label titleLabel = new WinFormsLabelBuilder().SetLocation(new Point(5, yLocation)).SetText(title).SetForeColor(Color.White).SetWidth(300).SetHeight(30);
        usernamesScrollablePanel.Controls.Add(titleLabel);
        yLocation += 30;
        int xLocation = 5;
        int count = usernameList.Count(); // Map the usernames onto the scroll panel
        int index = count - usernameList.Count();
        while (index < count)
        {
            Button button = new WinFormsButtonBuilder().SetText(usernameList[index]).SetLocation(new Point(xLocation, yLocation)).SetSize(new Size(120, 30)).SetBackColor(Color.IndianRed).SetForeColor(Color.White);
            button.Click += (sender, args) => // Add a click event handler to the button
            {
                Process.Start(new ProcessStartInfo($"https://x.com/{button.Text.Trim()}") { UseShellExecute = true }); // Open the default web browser
                Thread.Sleep(1000);
                ((Button)sender).BackColor = Color.Green;
            };
            usernamesScrollablePanel.Controls.Add(button); // Add the button to the Panel
            xLocation += 135;  // Update the positioning for the following button
            if ((index + 1) % 3 == 0)
            {
                xLocation = 5;
                yLocation += 30;
            }
            index += 1;
        }
        return yLocation;
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
