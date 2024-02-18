using System.Text.Json;

namespace PriceChecker2;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        ColView.BindingContext = PartDatabase.Instance;
    }
}