public class WinFormsPanelBuilder : Panel
{
    public WinFormsPanelBuilder SetLocation(Point location)
    {
        this.Location = location;
        return this;
    }

    public WinFormsPanelBuilder SetSize(Size size)
    {
        this.Size = size;
        return this;
    }

    public WinFormsPanelBuilder SetAutoScroll(bool autoScroll)
    {
        this.AutoScroll = autoScroll;
        return this;
    }
}