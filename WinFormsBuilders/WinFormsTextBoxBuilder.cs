public class WinFormsTextBoxBuilder : TextBox
{
    public WinFormsTextBoxBuilder SetText(string text)
    {
        this.Text = text;
        return this;
    }

    public WinFormsTextBoxBuilder SetLocation(Point location)
    {
        this.Location = location;
        return this;
    }

    public WinFormsTextBoxBuilder SetSize(Size size)
    {
        this.Size = size;
        return this;
    }

    public WinFormsTextBoxBuilder SetBackColor(Color color)
    {
        this.BackColor = color;
        return this;
    }

    public WinFormsTextBoxBuilder SetForeColor(Color color)
    {
        this.ForeColor = color;
        return this;
    }

    public WinFormsTextBoxBuilder SetTextAlign(HorizontalAlignment textAlign)
    {
        this.TextAlign = textAlign;
        return this;
    }
}