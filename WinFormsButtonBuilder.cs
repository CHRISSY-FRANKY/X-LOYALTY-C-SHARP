public class WinFormsButtonBuilder : Button
{
    public WinFormsButtonBuilder SetText(string text)
    {
        this.Text = text;
        return this;
    }

    public WinFormsButtonBuilder SetTextAlign(ContentAlignment textAlign)
    {
        this.TextAlign = textAlign;
        return this;
    }

    public WinFormsButtonBuilder SetLocation(Point location)
    {
        this.Location = location;
        return this;
    }

    public WinFormsButtonBuilder SetForeColor(Color color)
    {
        this.ForeColor = color;
        return this;
    }

    public WinFormsButtonBuilder SetBackColor(Color color)
    {
        this.BackColor = color;
        return this;
    }

    public WinFormsButtonBuilder SetSize(Size size)
    {
        this.Size = size;
        return this;
    }
}