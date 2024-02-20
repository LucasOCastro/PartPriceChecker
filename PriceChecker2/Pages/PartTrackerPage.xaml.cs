using PriceChecker2.Parts;

namespace PriceChecker2;

public partial class PartTrackerPage : ContentPage
{
    private async Task LoadPartInfoAsync()
    {
        IsBusy = true;
        await AsyncUtils.WaitUntil(() => PartDatabase.Instance.AllLoaded);
        _partsCollectionView.ItemsSource = PartDatabase.Instance.Parts;
        IsBusy = false;
    }

    public PartTrackerPage()
	{
		InitializeComponent();
        BindingContext = this;
        LoadPartInfoAsync();
    }
}