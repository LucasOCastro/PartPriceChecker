using PriceChecker2.Parts;

namespace PriceChecker2;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        BindingContext = this;
        InitializeComponent();
    }


    private async Task ClearAsync()
    {
        IsBusy = true;
        await PartDatabase.Instance.ClearAsync();
        IsBusy = false;
    }
    
    private void ClearButton_Pressed(object sender, EventArgs e)
    {
        ClearAsync();
    }
}