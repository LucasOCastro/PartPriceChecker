using PriceChecker2.Parts;
using PriceChecker2.UrlScraping;

namespace PriceChecker2.Pages;

public partial class PartViewingPage : ContentPage, IQueryAttributable
{
    public PartInfo? Part
    {
        get => GetValue(PartProperty) as PartInfo;
        set
        {
            SetValue(PartProperty, value);
            BindingContext = Part;

            _urlCollectionView.ItemsSource = SortedUrls;
        }
    }
    public readonly static BindableProperty PartProperty =
        BindableProperty.Create(nameof(Part), typeof(PartInfo), typeof(PartViewingPage));

    private IEnumerable<UrlScrapedData> SortedUrls => Part.AllUrlData.OrderBy(d=> d.IsValid ? d.Price : double.MaxValue);

    public PartViewingPage()
    {
        InitializeComponent();
    }

    private void EditButton_Pressed(object sender, EventArgs e)
        => ShellNavigator.Instance.NavigateAsync("//editor", new() { { nameof(PartEditingPage.Part), Part } });

    private void BackButton_Pressed(object sender, EventArgs e) => ShellNavigator.Instance.BackAsync();
    protected override bool OnBackButtonPressed()
    {
        ShellNavigator.Instance.BackAsync();
        return true;
    }

    private void Link_Tapped(object sender, TappedEventArgs args)
    {
        if (args.Parameter is not string url || !Uri.IsWellFormedUriString(url, UriKind.Absolute)) return;
        Launcher.OpenAsync(url);
    }

    private async Task DeleteAsync()
    {
        bool accept = await DisplayAlert("Are you sure?", "This will delete the part permanently.", "Accept", "Cancel");
        if (!accept) return;

        IsBusy = true;
        await PartDatabase.Instance.UnregisterAsync(Part.Part);
        IsBusy = false;
        await ShellNavigator.Instance.BackAsync();
    }
    private void DeleteButton_Pressed(object sender, EventArgs e) => DeleteAsync();

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue(nameof(Part), out var obj) || obj is not PartInfo part) return;
        Part = part;
    }
}