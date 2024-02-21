using PriceChecker2.Parts;

namespace PriceChecker2.Pages;

public partial class MainPage : ContentPage
{
    public Build Build { get; } = new();
    public string MoneyString { get; set; }

    public MainPage()
    {
        InitializeComponent();
        LoadPartInfoAsync();
    }

    private async Task LoadPartInfoAsync()
    {
        IsBusy = true;

        await AsyncUtils.WaitUntil(() => PartDatabase.Instance.AllLoaded);
        //_collectionView.ItemsSource = Build.BuildParts;

        IsBusy = false;
    }
}