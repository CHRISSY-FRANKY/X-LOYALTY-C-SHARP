
public class XLoyaltyFormBuilder
{
    private Form form;

    public XLoyaltyFormBuilder(string text, System.Drawing.Size formSize)
    {
        // Set application settings
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        // Create form based on parameters provided
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

    public Form GetForm()
    {
        return this.form;
    }


}
