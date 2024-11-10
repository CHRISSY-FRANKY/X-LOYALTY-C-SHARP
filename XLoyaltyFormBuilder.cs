// X Loyalty Form Builder Class to improve readability
using Microsoft.Extensions.Hosting;

public class XLoyaltyFormBuilder
{
    private Form form;
    private Label introLabel;
    private TextBox usernameTextBox;
    private PictureBox elonFrowningPictureBox;
    private PictureBox elonSmilingPictureBox;
    private PictureBox elonGoFYourselfPictureBox;
    private Button submitTryAgainButton;

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


    // Add a button to submit username and to try again
    public XLoyaltyFormBuilder AddSubmitTryAgainButton(Point location, Size size, Func<string, Task<XLoyaltyResponseCode>> VerifyUsername, IHost? host)
    {
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
                switch (response)
                {
                    // Show elon smiling
                    case XLoyaltyResponseCode.UsernameExists:
                        elonFrowningPictureBox.Visible = false;
                        elonSmilingPictureBox.Visible = true;
                        elonGoFYourselfPictureBox.Visible = false;
                        introLabel.Text = "\nWelcome to X LOYALTY!\nRedirecting you to x.com!\nPlease sign in within the next 30 seconds.";
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
