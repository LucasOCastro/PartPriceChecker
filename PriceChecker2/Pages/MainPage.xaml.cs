using PriceChecker2.Parts;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PriceChecker2;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        BindingContext = this;
        InitializeComponent();
        LoadPartInfoAsync();
    }

    private async Task LoadPartInfoAsync()
    {
        IsBusy = true;
        await AsyncUtils.WaitUntil(() => PartDatabase.Instance.AllLoaded);
        _partsCollectionView.ItemsSource = PartDatabase.Instance.Parts;
        IsBusy = false;
    }

    private void ClearButton_Pressed(object sender, EventArgs e) => ClearAsync();
    private async Task ClearAsync()
    {
        IsBusy = true;
        await PartDatabase.Instance.ClearAsync();
        IsBusy = false;
    }
}