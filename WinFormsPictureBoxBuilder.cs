public class WinFormsPictureBoxBuilder : PictureBox
{
    public WinFormsPictureBoxBuilder SetImage(Image image)
    {
        this.Image = image;
        return this;
    }

    public WinFormsPictureBoxBuilder SetLocation(Point location)
    {
        this.Location = location;
        return this;
    }

    public WinFormsPictureBoxBuilder SetSize(Size size)
    {
        this.Size = size;
        return this;
    }

    public WinFormsPictureBoxBuilder SetSizeMode(PictureBoxSizeMode sizeMode)
    {
        this.SizeMode = sizeMode;
        return this;
    }

    public WinFormsPictureBoxBuilder SetVisible(bool visible)
    {
        this.Visible = visible;
        return this;
    }
}