using System.Globalization;

namespace PriceChecker2;

public partial class App
{
    public App()
    {
        InitializeComponent();
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var page = new AppShell();
        return new Window(page);
    }
}