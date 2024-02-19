using PriceChecker2.Parts;

namespace PriceChecker2;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        BindingContext = this;
        InitializeComponent();
    }

    private void ClearButton_Pressed(object sender, EventArgs e)
    {
        IsBusy = true;
        Task.Run(async () => {
            await PartDatabase.Instance.ClearAsync();
            IsBusy = false;
        });
    }
}