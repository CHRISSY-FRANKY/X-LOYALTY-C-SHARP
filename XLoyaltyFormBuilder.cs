// X Loyalty Form Builder Class to improve readability
public class XLoyaltyFormBuilder
{
    private Form form;
    private Label introLabel;
    private TextBox usernameTextBox;
    private PictureBox elonFrowningPictureBox;
    private PictureBox elonSmilingPictureBox;
    private PictureBox elonGoFYourselfPictureBox;

    public XLoyaltyFormBuilder(string text, Size formSize)
    {
        // Set application settings
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        // Create form based on arguments provided
        this.form = new Form
        {
            Text = text,
            MaximizeBox = false,
            MinimizeBox = false,
            FormBorderStyle = FormBorderStyle.FixedSingle,
            BackColor = Color.Black,
            Size = formSize
        };
    }

    // Adds Intro Label to Form
    public XLoyaltyFormBuilder AddIntroLabel(string text, Point location, Size size)
    {
        // Create the label
        this.introLabel = new()
        {
            Text = text,
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
    public XLoyaltyFormBuilder AddUsernameTextBox(string text, Point location, Size size)
    {
        // Add the text box to the form
        TextBox usernameTextBox = new()
        {
            Text = text,
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
        return this;
    }
    
    public Form GetForm()
    {
        return this.form;
    }


}
