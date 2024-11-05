// X Loyalty Form Builder Class to improve readability
public class XLoyaltyFormBuilder
{
    private Form form;

    public XLoyaltyFormBuilder(string text, System.Drawing.Size formSize)
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
            BackColor = System.Drawing.Color.Black,
            Size = formSize
        };
    }

    public XLoyaltyFormBuilder AddIntroLabel(string text, System.Drawing.Point location, System.Drawing.Size size)
    {
        // Create the label
        Label label = new Label
        {
            Text = text,
            Location = location,
            Size = size,
            BackColor = System.Drawing.Color.Black,
            ForeColor = System.Drawing.Color.White,
            TextAlign = ContentAlignment.TopCenter
        };
        form.Controls.Add(label);
        return this;
    }



    public Form GetForm()
    {
        return this.form;
    }


}
