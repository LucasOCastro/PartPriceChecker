using PriceChecker2.UrlScraping;
using System.Collections.ObjectModel;

namespace PriceChecker2;

public partial class MainPage : ContentPage
{
    private readonly ObservableCollection<PartInfo> _scrapedParts = new();

    private async Task LoadPartInfoAsync(IEnumerable<Part> parts)
    {
        var scraped = parts.Select(p => new PartInfo(p)).ToList();
        while (scraped.Any(s => s.Loading))
            await Task.Delay(50);

        foreach (var s in scraped)
            _scrapedParts.Add(s);
    }

    public MainPage()
    {
        InitializeComponent();
        _partsCollectionView.ItemsSource = _scrapedParts;
        LoadPartInfoAsync(PartDatabase.Instance.Parts);
    }
}