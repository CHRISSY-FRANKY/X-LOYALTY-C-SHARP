// X Loyalty Form Builder Class to improve readability
public class XLoyaltyFormBuilder
{
    private Form form;

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

    public XLoyaltyFormBuilder AddIntroLabel(string text, Point location, Size size)
    {
        // Create the label
        Label label = new()
        {
            Text = text,
            Location = location,
            Size = size,
            BackColor = Color.Black,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.TopCenter
        };
        form.Controls.Add(label);
        return this;
    }

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

    public Form GetForm()
    {
        return this.form;
    }


}
