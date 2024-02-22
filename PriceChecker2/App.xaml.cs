using System.Globalization;

namespace PriceChecker2;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        CultureInfo.CurrentCulture = new("en-US");
        MainPage = new AppShell();
    }
}