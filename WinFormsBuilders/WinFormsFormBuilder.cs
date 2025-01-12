public class WinFormsFormBuilder : Form 
{
    public WinFormsFormBuilder SetText(string text)
    {
        this.Text = text;
        return this;
    }

    public WinFormsFormBuilder SetMaximizeBox(bool maximizeBox)
    {
        this.MaximizeBox = maximizeBox;
        return this;
    }

    public WinFormsFormBuilder SetMinimizeBox(bool minimizeBox)
    {
        this.MinimizeBox = minimizeBox;
        return this;
    }

    public WinFormsFormBuilder setFormBorderStyle(FormBorderStyle formBorderStyle)
    {
        this.FormBorderStyle = formBorderStyle;
        return this;
    }

    public WinFormsFormBuilder setBackColor(Color color)
    {
        this.BackColor = color;
        return this;
    }

    public WinFormsFormBuilder setSize(Size size)
    {
        this.Size = size;
        return this;
    }
}