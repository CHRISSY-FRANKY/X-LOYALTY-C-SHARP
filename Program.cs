using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        // Create XLoyaltyHost
        XLoyaltyHost xLoyaltyHost = new XLoyaltyHost(args, 5115, "secrets.json").BuildHost().RunHost();

        // Create the form builder to build the x loyalty form
        XLoyaltyFormBuilder form = new XLoyaltyFormBuilder(new Size(420, 420))
            .AddIntroLabel(new Point(0, 0), new Size(420, 60))
            .AddUsernameTextBox(new Point(105, 75), new Size(210, 180))
            .AddElonSmilingPictureBox("elonSmiling.jpg", new Point(0, 180), new Size(420, 200))
            .AddElonFrowningPictureBox("elonFrowning.jpeg", new Point(0, 180), new Size(420, 200))
            .AddElonGoFYourselfPictureBox("elonFYourself.jpg", new Point(0, 180), new Size(420, 200))
            .AddSubmitTryAgainButton(new Point(180, 120), new Size(60, 30), xLoyaltyHost.GetHost())
            .AddFollowersFollowingScrollControl()
            .Run();
    }


}