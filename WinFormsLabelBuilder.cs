public class WinFormsLabelBuilder : Label
{

    public WinFormsLabelBuilder SetText(string text)
    {
        this.Text = text;
        return this;
    }

    public WinFormsLabelBuilder SetLocation(Point location)
    {
        this.Location = location;
        return this;
    }

    public WinFormsLabelBuilder SetSize(Size size)
    {
        this.Size = size;
        return this;
    }

    public WinFormsLabelBuilder SetBackColor(Color color)
    {
        this.BackColor = color;
        return this;
    }

    public WinFormsLabelBuilder SetForeColor(Color color)
    {
        this.ForeColor = color;
        return this;
    }

    public WinFormsLabelBuilder SetTextAlign(ContentAlignment contentAlignment)
    {
        this.TextAlign = contentAlignment;
        return this;
    }

    public WinFormsLabelBuilder SetWidth(int width)
    {
        this.Width = width;
        return this;
    }

    public WinFormsLabelBuilder SetHeight(int height)
    {
        this.Height = height;
        return this;
    }
}